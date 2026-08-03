using System.Threading.Channels;
using SimpleBus;

namespace CoConnect.Infrastructure.Queue
{
    public class MessageQueue
    {
        private readonly Channel<IMessage> _queue = Channel.CreateUnbounded<IMessage>();

        public async Task EnqueueAsync(IMessage message)
        {
            await _queue.Writer.WriteAsync(message);
        }

        public async Task<IMessage> DequeueAsync(CancellationToken token)
        {
            return await _queue.Reader.ReadAsync(token);
        }
    }
}
