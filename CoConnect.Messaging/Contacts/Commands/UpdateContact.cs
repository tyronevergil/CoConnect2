namespace CoConnect.Messaging.Contacts.Commands
{
    public class UpdateContact : CommandMessageBase
    {
        public string ContactId { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

