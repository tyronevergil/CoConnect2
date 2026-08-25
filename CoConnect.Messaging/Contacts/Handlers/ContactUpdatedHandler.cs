using System.Threading.Tasks;
using CoConnect.Messaging.Contacts.Events;
using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Messaging.Contacts.Handlers
{
    public class ContactUpdatedHandler : IMessageHandler<ContactUpdated>
    {
        private readonly ILogger<ContactUpdatedHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;

        public ContactUpdatedHandler(ILogger<ContactUpdatedHandler> logger, INotificationDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task Handle(IServiceContext context, ContactUpdated message)
        {
            _logger.LogInformation("Processing ContactUpdated event: ContactId={ContactId}", message.ContactId);
            await _dispatcher.PublishAsync("updated.contact", message);
            _logger.LogInformation("Successfully dispatched updated.contact notification to client dispatcher for ContactId={ContactId}", message.ContactId);
        }
    }
}
