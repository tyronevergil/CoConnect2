using CoConnect.Messaging.Abstractions;

namespace CoConnect.Messaging
{
    public class UserDisable : CommandMessageBase
    {
        public string UserId { get; set; } = string.Empty;
    }
}

