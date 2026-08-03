using CoConnect.Domain.Abstractions;

namespace CoConnect.Domain
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
