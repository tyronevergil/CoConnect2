
using SimpleBus;

namespace CoConnect.Infrastructure.Queue
{
    public class MessageQueueWorker : BackgroundService
    {
        private readonly ILogger<MessageQueueWorker> _logger;
        private readonly MessageQueue _queue;

        private readonly IServiceProcessor _processor;

        public MessageQueueWorker(ILogger<MessageQueueWorker> logger, MessageQueue queue, IServiceProcessor processor)
        {
            _logger = logger;
            _queue = queue;
            _processor = processor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queue worker started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _queue.DequeueAsync(stoppingToken);

                try
                {
                    _logger.LogInformation($"Processing message: {message}");
                    
                    await _processor.Process(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                }
            }
        }
    }
}
