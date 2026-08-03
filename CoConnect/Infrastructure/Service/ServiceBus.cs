using System.Runtime.CompilerServices;
using SimpleBus;
using CoConnect.Infrastructure.Queue;

namespace CoConnect.Infrastructure.Service
{
    public class ServiceBus : IServiceBus, IServiceContext
    {
        private readonly ILogger<ServiceBus> _logger;
        private readonly MessageQueue _queue;

        public ServiceBus(ILogger<ServiceBus> logger, MessageQueue queue)
        {
            _logger = logger;
            _queue = queue;
        }

        public async Task Send<T>(T message) where T : ICommandMessage
        {
            await _queue.EnqueueAsync(message);
        }

        public async Task Publish<T>(T message) where T : IEventMessage
        {
            await _queue.EnqueueAsync(message);
        }

        //private Guid NewSequentialGuid()
        //{
        //    var guidArray = Guid.NewGuid().ToByteArray();
        //    var now = DateTime.UtcNow;

        //    // Get bytes from timestamp
        //    byte[] days = BitConverter.GetBytes(now.Ticks / TimeSpan.TicksPerDay);
        //    byte[] msecs = BitConverter.GetBytes(now.Ticks % TimeSpan.TicksPerDay);

        //    // Adjust for endianness
        //    if (BitConverter.IsLittleEndian)
        //    {
        //        Array.Reverse(days);
        //        Array.Reverse(msecs);
        //    }

        //    // Replace first 8 bytes with timestamp
        //    Array.Copy(days, days.Length - 2, guidArray, 0, 2);
        //    Array.Copy(msecs, msecs.Length - 4, guidArray, 2, 4);

        //    return new Guid(guidArray);
        //}
    }
}
