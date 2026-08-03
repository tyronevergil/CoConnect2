using CoConnect.Domain.Abstractions;

namespace CoConnect.Domain
{
    public class ContactUpdated : EventMessageBase
    {
        public string ContactId { get; set; }
    }
}
