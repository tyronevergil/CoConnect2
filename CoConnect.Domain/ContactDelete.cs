using CoConnect.Domain.Abstractions;

namespace CoConnect.Domain
{
    public class ContactDelete : CommandMessageBase
    {
        public string ContactId { get; set; }
    }
}
