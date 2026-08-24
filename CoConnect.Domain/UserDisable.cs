using CoConnect.Domain.Abstractions;

namespace CoConnect.Domain
{
    public class UserDisable : CommandMessageBase
    {
        public string UserId { get; set; } = string.Empty;
    }
}
