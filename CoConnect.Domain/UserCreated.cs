using CoConnect.Domain.Abstractions;

namespace CoConnect.Domain
{
    public class UserCreated : EventMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    }
}
