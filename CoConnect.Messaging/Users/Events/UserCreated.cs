namespace CoConnect.Messaging.Users.Events
{
    public class UserCreated : EventMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    }
}

