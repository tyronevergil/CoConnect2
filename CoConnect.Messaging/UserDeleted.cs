using CoConnect.Messaging.Abstractions;

namespace CoConnect.Messaging
{
    public class UserDeleted : EventMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Reason { get; set; } = "Your account was removed.";
    }
}

