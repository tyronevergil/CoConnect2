using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Persistence;
using Persistence.Specifications;
using SimpleBus;

namespace CoConnect.Domain.Handlers
{
    public class UserUpdateHandler : IMessageHandler<UserUpdate>
    {
        private readonly ILogger<UserUpdateHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IDataContextFactory _factory;

        public UserUpdateHandler(ILogger<UserUpdateHandler> logger, INotificationDispatcher dispatcher, IDataContextFactory factory)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _factory = factory;
        }

        public async Task Handle(IServiceContext context, UserUpdate message)
        {
            try
            {
                using var dataContext = _factory.CreateDataContext();
                var user = await dataContext.FindSingleAsync(UserSpecs.Get(message.UserId));
                if (user == null)
                {
                    throw new ApplicationException($"User with id '{message.UserId}' not found.");
                }

                user.Username = message.Username;
                user.Role = message.Role;
                user.IsDisabled = message.IsDisabled;

                var requiresSessionRefresh = false;
                if (!string.IsNullOrWhiteSpace(message.Password))
                {
                    user.PasswordHash = HashPassword(message.Password);
                    requiresSessionRefresh = true;
                }

                user.SecurityStamp = Guid.NewGuid().ToString("N");
                user.UpdatedUtc = DateTimeOffset.UtcNow;

                dataContext.Update(user);
                await dataContext.SaveChangesAsync();

                await context.Publish(new UserUpdated
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    RequiresSessionRefresh = requiresSessionRefresh || user.IsDisabled,
                    ConnectionId = message.ConnectionId,
                    TransactionId = message.TransactionId
                });
            }
            catch (Exception ex)
            {
                var error = $"Error processing UserUpdate command for UserId={message.UserId}";
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
