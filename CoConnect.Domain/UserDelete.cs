using CoConnect.Domain.Abstractions;

namespace CoConnect.Domain
{
    public class UserDelete : CommandMessageBase
    {
        public string UserId { get; set; } = string.Empty;
    }
}
