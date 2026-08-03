using Microsoft.Extensions.Logging;
using SimpleBus;

namespace CoConnect.Infrastructure.Service
{
    public class ServiceProcessor : IServiceProcessor
    {
        private readonly ILogger<ServiceProcessor> _logger;
        private readonly IEnumerable<IMessageHandler> _handlers;
        private readonly IServiceContext _context;

        public ServiceProcessor(ILogger<ServiceProcessor> logger, IEnumerable<IMessageHandler> handlers, IServiceContext context)
        {
            _logger = logger;
            _handlers = handlers;
            _context = context;
        }

        public async Task Process(IMessage message)
        {
            var messageType = message.GetType();
            var handlerType = typeof(IMessageHandler<>).MakeGenericType(messageType);

            var tasks = _handlers
                .Where(h => handlerType.IsAssignableFrom(h.GetType()))
                .Select(handler =>
                {
                    return Task.Run(() =>
                    {
                        var method = handlerType.GetMethod("Handle");
                        if (method != null) 
                            method.Invoke(handler, [ _context, message ]);
                    });
                });

            await Task.WhenAll(tasks);
        }
    }
}
