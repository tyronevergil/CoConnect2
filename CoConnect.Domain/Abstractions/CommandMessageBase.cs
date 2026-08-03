using SimpleBus;

namespace CoConnect.Domain.Abstractions
{
    public abstract class CommandMessageBase : ICommandMessage
    {
        public string TransactionId { get; set; }

        public string ConnectionId { get; set; }

        public Dictionary<string, object> Payload { get; set; } = new();
    }
}
