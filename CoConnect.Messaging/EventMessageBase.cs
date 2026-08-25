using SimpleBus;

namespace CoConnect.Messaging
{
    public class EventMessageBase : IEventMessage
    {
        public string TransactionId { get; set; }

        public string ConnectionId { get; set; }
    }
}

