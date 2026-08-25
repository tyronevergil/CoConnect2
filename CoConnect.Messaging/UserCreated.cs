using CoConnect.Messaging.Abstractions;

namespace CoConnect.Messaging
{
    public class UserCreated : EventMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    }
}

