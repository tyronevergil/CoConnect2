using Microsoft.Extensions.Logging;
using Persistence;
using Persistence.Specifications;
using SimpleBus;

namespace CoConnect.Domain.Handlers
{
    public class UserDisableHandler : IMessageHandler<UserDisable>
    {
        private readonly ILogger<UserDisableHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IDataContextFactory _factory;

        public UserDisableHandler(ILogger<UserDisableHandler> logger, INotificationDispatcher dispatcher, IDataContextFactory factory)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _factory = factory;
        }

        public async Task Handle(IServiceContext context, UserDisable message)
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
                var error = $"Error processing UserDisable command for UserId={message.UserId}";
                _logger.LogError(ex, error);
                await _dispatcher.PublishAsync(message.ConnectionId, "app.error", new { message = error, exception = ex.Message });
            }
        }
    }
}
