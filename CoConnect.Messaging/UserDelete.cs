using CoConnect.Messaging.Abstractions;

namespace CoConnect.Messaging
{
    public class UserDelete : CommandMessageBase
    {
        public string UserId { get; set; } = string.Empty;
    }
}

