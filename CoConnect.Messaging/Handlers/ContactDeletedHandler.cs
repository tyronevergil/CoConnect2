using SimpleBus;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CoConnect.Messaging.Handlers
{
    public class ContactDeletedHandler : IMessageHandler<ContactDeleted>
    {
        private readonly ILogger<ContactDeletedHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;

        public ContactDeletedHandler(ILogger<ContactDeletedHandler> logger, INotificationDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task Handle(IServiceContext context, ContactDeleted message)
        {
            _logger.LogInformation("Processing ContactDeleted event: ContactId={ContactId}", message.ContactId);
            await _dispatcher.PublishAsync("deleted.contact", message);
            _logger.LogInformation("Successfully dispatched deleted.contact notification to client dispatcher for ContactId={ContactId}", message.ContactId);
        }
    }
}
