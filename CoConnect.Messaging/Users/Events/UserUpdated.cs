namespace CoConnect.Messaging.Users.Events
{
    public class UserUpdated : EventMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public bool RequiresSessionRefresh { get; set; }
    }
}

