using CoConnect.Messaging.Users.Commands;
using CoConnect.Messaging.Users.Events;
using CoConnect.Persistence;
using CoConnect.Persistence.Specifications;
using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Messaging.Users.Handlers
{
    public class DeleteUserHandler : IMessageHandler<DeleteUser>
    {
        private readonly ILogger<DeleteUserHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IDataContextFactory _factory;

        public DeleteUserHandler(ILogger<DeleteUserHandler> logger, INotificationDispatcher dispatcher, IDataContextFactory factory)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _factory = factory;
        }

        public async Task Handle(IServiceContext context, DeleteUser message)
        {
            try
            {
                using var dataContext = _factory.CreateDataContext();
                var user = await dataContext.FindSingleAsync(UserSpecs.Get(message.UserId));
                if (user == null)
                {
                    throw new ApplicationException($"User with id '{message.UserId}' not found.");
                }

                var userId = user.UserId;
                var username = user.Username;

                dataContext.Delete(user);
                await dataContext.SaveChangesAsync();

                await context.Publish(new UserDeleted
                {
                    UserId = userId,
                    Username = username,
                    ConnectionId = message.ConnectionId,
                    TransactionId = message.TransactionId
                });
            }
            catch (Exception ex)
            {
                var error = $"Error processing DeleteUser command for UserId={message.UserId}";
                _logger.LogError(ex, error);
                await _dispatcher.PublishAsync(message.ConnectionId, "app.error", new { message = error, exception = ex.Message });
            }
        }
    }
}

