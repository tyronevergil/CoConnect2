using CoConnect.Messaging.Users.Commands;
using CoConnect.Messaging.Users.Events;
using CoConnect.Persistence;
using CoConnect.Persistence.Specifications;
using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Messaging.Users.Handlers
{
    public class DisableUserHandler : IMessageHandler<DisableUser>
    {
        private readonly ILogger<DisableUserHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IDataContextFactory _factory;

        public DisableUserHandler(ILogger<DisableUserHandler> logger, INotificationDispatcher dispatcher, IDataContextFactory factory)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _factory = factory;
        }

        public async Task Handle(IServiceContext context, DisableUser message)
        {
            try
            {
                using var dataContext = _factory.CreateDataContext();
                var user = await dataContext.FindSingleAsync(UserSpecs.Get(message.UserId));
                if (user == null)
                {
                    throw new ApplicationException($"User with id '{message.UserId}' not found.");
                }

                user.IsDisabled = true;
                user.SecurityStamp = Guid.NewGuid().ToString("N");
                user.UpdatedUtc = DateTimeOffset.UtcNow;

                dataContext.Update(user);
                await dataContext.SaveChangesAsync();

                await context.Publish(new UserDisabled
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    ConnectionId = message.ConnectionId,
                    TransactionId = message.TransactionId
                });
            }
            catch (Exception ex)
            {
                var error = $"Error processing DisableUser command for UserId={message.UserId}";
                _logger.LogError(ex, error);
                await _dispatcher.PublishAsync(message.ConnectionId, "app.error", new { message = error, exception = ex.Message });
            }
        }
    }
}

