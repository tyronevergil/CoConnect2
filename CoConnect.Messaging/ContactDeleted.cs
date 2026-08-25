using CoConnect.Messaging.Abstractions;

namespace CoConnect.Messaging
{
    public class ContactDeleted : EventMessageBase
    {
        public string ContactId { get; set; }
    }
}

