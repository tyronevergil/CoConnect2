using System.Threading.Tasks;
using CoConnect.Messaging.Contacts.Events;
using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Messaging.Contacts.Handlers
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
