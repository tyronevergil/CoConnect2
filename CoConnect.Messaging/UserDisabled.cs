using CoConnect.Messaging.Abstractions;

namespace CoConnect.Messaging
{
    public class UserDisabled : EventMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Reason { get; set; } = "Your account was disabled.";
    }
}

