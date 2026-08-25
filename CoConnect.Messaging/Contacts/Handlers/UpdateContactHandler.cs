using System;
using System.Threading.Tasks;
using CoConnect.Messaging.Contacts.Commands;
using CoConnect.Messaging.Contacts.Events;
using CoConnect.Persistence;
using CoConnect.Persistence.Specifications;
using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Messaging.Contacts.Handlers
{
    public class UpdateContactHandler : IMessageHandler<UpdateContact>
    {
        private readonly ILogger<UpdateContactHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IDataContextFactory _factory;

        public UpdateContactHandler(ILogger<UpdateContactHandler> logger, INotificationDispatcher dispatcher, IDataContextFactory factory)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _factory = factory;
        }

        public async Task Handle(IServiceContext context, UpdateContact message)
        {
            try
            {
                _logger.LogInformation("Processing UpdateContact command for ContactId={ContactId}: Firstname={Firstname}, Lastname={Lastname}", message.ContactId, message.Firstname, message.Lastname);

                using (var dataContext = _factory.CreateDataContext())
                {
                    var contact = await dataContext.FindSingleAsync(ContactSpecs.Get(message.ContactId));
                    if (contact == null)
                    {
                        var error = string.Format("Contact with Id={ContactId} not found", message.ContactId);
                        _logger.LogWarning(error);
                        throw new ApplicationException(error);
                    }
                    contact.Firstname = message.Firstname;
                    contact.Lastname = message.Lastname;
                    contact.Email = message.Email;
                    contact.Phone = message.Phone;
                    dataContext.Update(contact);
                    await dataContext.SaveChangesAsync();
                }

                await context.Publish(new ContactUpdated
                {
                    ContactId = message.ContactId,
                });

                _logger.LogInformation("Successfully published ContactUpdated event for Id={ContactId}", message.ContactId);
            }
            catch (Exception ex)
            {
                var error = string.Format("Error processing UpdateContact command for ContactId={0}: Firstname={1}, Lastname={2}", message.ContactId, message.Firstname, message.Lastname);
                _logger.LogError(ex, error);
                await _dispatcher.PublishAsync(message.ConnectionId, "app.error", new { message = error, exception = ex.Message });
            }
        }
    }
}
