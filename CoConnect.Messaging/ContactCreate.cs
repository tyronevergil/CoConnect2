using CoConnect.Messaging.Abstractions;

namespace CoConnect.Messaging
{
    public class ContactCreate : CommandMessageBase
    {
        public string ContactId { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

