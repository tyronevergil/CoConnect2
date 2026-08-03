using CoConnect.Domain.Abstractions;

namespace CoConnect.Domain
{
    public class ContactDeleted : EventMessageBase
    {
        public string ContactId { get; set; }
    }
}
