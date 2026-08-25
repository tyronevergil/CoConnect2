using CoConnect.Messaging.Abstractions;

namespace CoConnect.Messaging
{
    public class ContactUpdated : EventMessageBase
    {
        public string ContactId { get; set; }
    }
}

