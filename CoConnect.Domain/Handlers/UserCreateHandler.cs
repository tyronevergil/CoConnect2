using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Persistence;
using Persistence.Entities;
using Persistence.Specifications;
using SimpleBus;

namespace CoConnect.Domain.Handlers
{
    public class UserCreateHandler : IMessageHandler<UserCreate>
    {
        private readonly ILogger<UserCreateHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IDataContextFactory _factory;

        public UserCreateHandler(ILogger<UserCreateHandler> logger, INotificationDispatcher dispatcher, IDataContextFactory factory)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _factory = factory;
        }

        public async Task Handle(IServiceContext context, UserCreate message)
        {
            try
            {
                message.UserId = Guid.NewGuid().ToString();

                using var dataContext = _factory.CreateDataContext();
                var existing = await dataContext.FindSingleAsync(UserSpecs.GetByUsername(message.Username));
                if (existing != null)
                {
                    throw new ApplicationException($"User with username '{message.Username}' already exists.");
                }

                var user = new User
                {
                    UserId = message.UserId,
                    Username = message.Username,
                    PasswordHash = HashPassword(message.Password),
                    Role = message.Role,
                    IsDisabled = message.IsDisabled,
                    SecurityStamp = Guid.NewGuid().ToString("N"),
                    UpdatedUtc = DateTimeOffset.UtcNow
                };

                dataContext.Add(user);
                await dataContext.SaveChangesAsync();

                await context.Publish(new UserCreated
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    ConnectionId = message.ConnectionId,
                    TransactionId = message.TransactionId
                });
            }
            catch (Exception ex)
            {
                var error = $"Error processing UserCreate command for Username={message.Username}";
                _logger.LogError(ex, error);
                await _dispatcher.PublishAsync(message.ConnectionId, "app.error", new { message = error, exception = ex.Message });
            }
        }

        private static string HashPassword(string password)
        {
            var value = password ?? string.Empty;
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(bytes);
        }
    }
}
