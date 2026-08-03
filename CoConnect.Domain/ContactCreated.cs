using CoConnect.Domain.Abstractions;

namespace CoConnect.Domain
{
    public class ContactCreated : EventMessageBase
    {
        public string ContactId { get; set; }
    }
}
