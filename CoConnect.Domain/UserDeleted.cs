using CoConnect.Domain.Abstractions;

namespace CoConnect.Domain
{
    public class UserDeleted : EventMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Reason { get; set; } = "Your account was removed.";
    }
}
