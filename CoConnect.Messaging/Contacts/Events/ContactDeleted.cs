namespace CoConnect.Messaging.Contacts.Events
{
    public class ContactDeleted : EventMessageBase
    {
        public string ContactId { get; set; }
    }
}

