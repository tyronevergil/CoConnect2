namespace CoConnect.Messaging.Users.Events
{
    public class UserDisabled : EventMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Reason { get; set; } = "Your account was disabled.";
    }
}

