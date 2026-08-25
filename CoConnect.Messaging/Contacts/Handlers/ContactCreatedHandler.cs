using System.Threading.Tasks;
using CoConnect.Messaging.Contacts.Events;
using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Messaging.Contacts.Handlers
{
    public class ContactCreatedHandler : IMessageHandler<ContactCreated>
    {
        private readonly ILogger<ContactCreatedHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;

        public ContactCreatedHandler(ILogger<ContactCreatedHandler> logger, INotificationDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task Handle(IServiceContext context, ContactCreated message)
        {
            _logger.LogInformation("Processing ContactCreated event: ContactId={ContactId}", message.ContactId);
            await _dispatcher.PublishAsync("created.contact", message);
            _logger.LogInformation("Successfully dispatched created.contact notification to client dispatcher for ContactId={ContactId}", message.ContactId);
        }
    }
}
