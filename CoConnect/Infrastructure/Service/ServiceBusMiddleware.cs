using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using CoConnect.Messaging;
using CoConnect.Messaging.Abstractions;
using SimpleBus;

namespace CoConnect.Infrastructure.Service
{
    public class ServiceBusMiddleware
    {
        private static readonly IEnumerable<Type> _messageTypes;
        private static readonly HashSet<string> _adminCommands = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(UserCreate),
            nameof(UserUpdate),
            nameof(UserDisable),
            nameof(UserDelete)
        };

        static ServiceBusMiddleware()
        {
            var messageType = typeof(ICommandMessage);
            var domainAssembly = typeof(INotificationDispatcher).Assembly;

            _messageTypes = domainAssembly
                .GetTypes()
                .Where(t => messageType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();
        }

        private readonly ILogger<ServiceBusMiddleware> _logger;
        private readonly IServiceBus _bus;
        private readonly RequestDelegate _next;

        public ServiceBusMiddleware(ILogger<ServiceBusMiddleware> logger, IServiceBus bus, RequestDelegate next)
        {
            _logger = logger;
            _bus = bus;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var req = context.Request;

                if (req.Path.StartsWithSegments("/api/command", StringComparison.OrdinalIgnoreCase, out PathString remaining))
                {
                    var modelType = _messageTypes
                        .FirstOrDefault(t => t.FullName != null && t.FullName.EndsWith(remaining.ToString().Replace("/", string.Empty), StringComparison.OrdinalIgnoreCase));

                    if (modelType != null)
                    {
                        if (!context.User.Identity?.IsAuthenticated ?? true)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }

                        if (_adminCommands.Contains(modelType.Name) && !context.User.IsInRole("Admin"))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return;
                        }

                        _logger.LogInformation("Processing command {Command}", modelType.Name);

                        var reader = new StreamReader(req.Body);
                        var jsonString = await reader.ReadToEndAsync();

                        var jsonObj = JsonSerializer.Deserialize(jsonString, modelType, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (jsonObj is CommandMessageBase commandMessage)
                        {
                            commandMessage.Payload["currentUserId"] = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                            commandMessage.Payload["currentUsername"] = context.User.Identity?.Name;

                            using var jsonDoc = JsonDocument.Parse(jsonString);
                            foreach (var property in jsonDoc.RootElement.EnumerateObject())
                            {
                                commandMessage.Payload[property.Name] = ConvertJsonElementToObject(property.Value);
                            }
                        }

                        if (jsonObj != null)
                        {
                            await _bus.Send((ICommandMessage)jsonObj);
                            context.Response.StatusCode = StatusCodes.Status202Accepted;
                            return;
                        }
                    }
                }

                await _next(context);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Invalid JSON payload for command request.");

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\":\"Invalid JSON payload.\"}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error while processing command request.");

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\":\"An unexpected error occurred while processing the request.\"}");
                }
            }
        }

        private static object? ConvertJsonElementToObject(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => ConvertJsonObjectToDictionary(element),
                JsonValueKind.Array => ConvertJsonArrayToList(element),
                JsonValueKind.String => ConvertJsonStringValue(element.GetString()),
                JsonValueKind.Number when element.TryGetInt32(out var i) => i,
                JsonValueKind.Number when element.TryGetInt64(out var l) => l,
                JsonValueKind.Number when element.TryGetDecimal(out var d) => d,
                JsonValueKind.Number when element.TryGetDouble(out var dbl) => dbl,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Undefined => null,
                _ => element.GetRawText()
            };
        }

        private static Dictionary<string, object?> ConvertJsonObjectToDictionary(JsonElement element)
        {
            var dictionary = new Dictionary<string, object?>();

            foreach (var property in element.EnumerateObject())
            {
                dictionary[property.Name] = ConvertJsonElementToObject(property.Value);
            }

            return dictionary;
        }

        private static List<object?> ConvertJsonArrayToList(JsonElement element)
        {
            var list = new List<object?>();

            foreach (var item in element.EnumerateArray())
            {
                list.Add(ConvertJsonElementToObject(item));
            }

            return list;
        }

        private static object? ConvertJsonStringValue(string? value)
        {
            if (value == null)
            {
                return null;
            }

            if (DateTimeOffset.TryParse(value, out var dto))
            {
                return dto;
            }

            if (DateTime.TryParse(value, out var dt))
            {
                return dt;
            }

            if (DateOnly.TryParse(value, out var dateOnly))
            {
                return dateOnly;
            }

            if (TimeOnly.TryParse(value, out var timeOnly))
            {
                return timeOnly;
            }

            return value;
        }
    }
}
