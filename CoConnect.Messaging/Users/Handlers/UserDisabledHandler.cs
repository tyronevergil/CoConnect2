using CoConnect.Messaging.Users.Events;
using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Messaging.Users.Handlers
{
    public class UserDisabledHandler : IMessageHandler<UserDisabled>
    {
        private readonly ILogger<UserDisabledHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;

        public UserDisabledHandler(ILogger<UserDisabledHandler> logger, INotificationDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task Handle(IServiceContext context, UserDisabled message)
        {
            _logger.LogInformation("Processing UserDisabled event: UserId={UserId}", message.UserId);
            await _dispatcher.PublishAsync("disabled.user", message);
            await _dispatcher.PublishAsync("app.signout", new
            {
                username = message.Username,
                reason = "disabled",
                message = message.Reason,
                redirectUrl = "/Account/Login"
            });
        }
    }
}

