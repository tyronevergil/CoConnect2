namespace CoConnect.Messaging.Users.Events
{
    public class UserDeleted : EventMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Reason { get; set; } = "Your account was removed.";
    }
}

