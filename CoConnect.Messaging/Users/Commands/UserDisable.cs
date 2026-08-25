namespace CoConnect.Messaging.Users.Commands
{
    public class UserDisable : CommandMessageBase
    {
        public string UserId { get; set; } = string.Empty;
    }
}

