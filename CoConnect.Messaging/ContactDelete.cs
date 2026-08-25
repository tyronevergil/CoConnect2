using CoConnect.Messaging.Abstractions;

namespace CoConnect.Messaging
{
    public class ContactDelete : CommandMessageBase
    {
        public string ContactId { get; set; }
    }
}

