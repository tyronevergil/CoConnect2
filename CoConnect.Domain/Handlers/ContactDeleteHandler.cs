using System;
using System.Threading.Tasks;
using SimpleBus;
using Persistence;
using Persistence.Specifications;
using Microsoft.Extensions.Logging;

namespace CoConnect.Domain.Handlers
{
    public class ContactDeleteHandler : IMessageHandler<ContactDelete>
    {
        private readonly ILogger<ContactDeleteHandler> _logger;
        private readonly INotificationDispatcher _dispatcher;
        private readonly IDataContextFactory _factory;

        public ContactDeleteHandler(ILogger<ContactDeleteHandler> logger, INotificationDispatcher dispatcher, IDataContextFactory factory)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _factory = factory;
        }

        public async Task Handle(IServiceContext context, ContactDelete message)
        {
            try
            {
                _logger.LogInformation("Processing ContactDelete command for ContactId={ContactId}", message.ContactId);

                using (var dataContext = _factory.CreateDataContext())
                {
                    var contact = await dataContext.FindSingleAsync(ContactSpecs.Get(message.ContactId));
                    if (contact == null)
                    {
                        var error = string.Format("Contact with Id={ContactId} not found", message.ContactId);
                        _logger.LogWarning(error);
                        throw new ApplicationException(error);
                    }
                    dataContext.Delete(contact);
                    await dataContext.SaveChangesAsync();
                }

                await context.Publish(new ContactDeleted
                {
                    ContactId = message.ContactId
                });

                _logger.LogInformation("Successfully published ContactDeleted event for Id={ContactId}", message.ContactId);
            }
            catch (Exception ex)
            {
                var error = string.Format("Error processing ContactDelete command for ContactId={ContactId}", message.ContactId);
                _logger.LogError(ex, error);
                await _dispatcher.PublishAsync(message.ConnectionId, "app.error", new { message = error, exception = ex.Message });
            }
        }
    }
}