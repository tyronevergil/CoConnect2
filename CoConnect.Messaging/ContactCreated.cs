using CoConnect.Messaging.Abstractions;

namespace CoConnect.Messaging
{
    public class ContactCreated : EventMessageBase
    {
        public string ContactId { get; set; }
    }
}

