using System;
using System.Threading.Tasks;
using SimpleBus;
using CoConnect.Persistence;
using CoConnect.Persistence.Entities;
using Microsoft.Extensions.Logging;

namespace CoConnect.Messaging.Handlers
{
    public class ContactCreateHandler : IMessageHandler<ContactCreate>
    {
        private readonly ILogger<ContactCreateHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IDataContextFactory _factory;

        public ContactCreateHandler(ILogger<ContactCreateHandler> logger, INotificationDispatcher dispatcher, IDataContextFactory factory)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _factory = factory;
        }

        public async Task Handle(IServiceContext context, ContactCreate message)
        {
            try
            {
                message.ContactId = Guid.NewGuid().ToString();

                _logger.LogInformation("Processing ContactCreate command: Firstname={Firstname}, Lastname={Lastname}, GeneratedId={ContactId}", message.Firstname, message.Lastname, message.ContactId);

                if (message.Firstname == "error")
                {
                    throw new InvalidOperationException("Simulated error");
                }

                using (var dataContext = _factory.CreateDataContext())
                {
                    var contact = new Contact
                    {
                        ContactId = message.ContactId,
                        Lastname = message.Lastname,
                        Firstname = message.Firstname,
                        Email = message.Email,
                        Phone = message.Phone
                    };
                    dataContext.Add(contact);
                    await dataContext.SaveChangesAsync();
                }

                await context.Publish(new ContactCreated
                {
                    ContactId = message.ContactId
                });

                _logger.LogInformation("Successfully published ContactCreated event for Id={ContactId}", message.ContactId);
            }
            catch (Exception ex)
            {
                var error = string.Format("Error processing ContactCreate command: Firstname={0}, Lastname={1}", message.Firstname, message.Lastname);
                _logger.LogError(ex, error);
                await _dispatcher.PublishAsync(message.ConnectionId, "app.error", new { message = error, exception = ex.Message });
            }
        }
    }
}
