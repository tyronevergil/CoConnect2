using SimpleBus;

namespace CoConnect.Messaging.Abstractions
{
    public class EventMessageBase : IEventMessage
    {
        public string TransactionId { get; set; }

        public string ConnectionId { get; set; }
    }
}

