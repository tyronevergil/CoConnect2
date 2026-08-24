using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Domain.Handlers
{
    public class UserCreatedHandler : IMessageHandler<UserCreated>
    {
        private readonly ILogger<UserCreatedHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;

        public UserCreatedHandler(ILogger<UserCreatedHandler> logger, INotificationDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task Handle(IServiceContext context, UserCreated message)
        {
            _logger.LogInformation("Processing UserCreated event: UserId={UserId}", message.UserId);
            await _dispatcher.PublishAsync("created.user", message);
        }
    }
}
