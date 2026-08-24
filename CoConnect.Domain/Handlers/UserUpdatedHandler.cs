using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Domain.Handlers
{
    public class UserUpdatedHandler : IMessageHandler<UserUpdated>
    {
        private readonly ILogger<UserUpdatedHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;

        public UserUpdatedHandler(ILogger<UserUpdatedHandler> logger, INotificationDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task Handle(IServiceContext context, UserUpdated message)
        {
            _logger.LogInformation("Processing UserUpdated event: UserId={UserId}", message.UserId);
            await _dispatcher.PublishAsync("updated.user", message);

            if (message.RequiresSessionRefresh)
            {
                await _dispatcher.PublishAsync("app.signout", new
                {
                    username = message.Username,
                    reason = "updated",
                    message = "Your account settings changed. Please sign in again.",
                    redirectUrl = "/Account/Login"
                });
            }
        }
    }
}
