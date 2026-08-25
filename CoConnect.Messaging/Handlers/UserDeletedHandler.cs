using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Messaging.Handlers
{
    public class UserDeletedHandler : IMessageHandler<UserDeleted>
    {
        private readonly ILogger<UserDeletedHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;

        public UserDeletedHandler(ILogger<UserDeletedHandler> logger, INotificationDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task Handle(IServiceContext context, UserDeleted message)
        {
            _logger.LogInformation("Processing UserDeleted event: UserId={UserId}", message.UserId);
            await _dispatcher.PublishAsync("deleted.user", message);
            await _dispatcher.PublishAsync("app.signout", new
            {
                username = message.Username,
                reason = "deleted",
                message = message.Reason,
                redirectUrl = "/Account/Login"
            });
        }
    }
}

