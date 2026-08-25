namespace CoConnect.Messaging.Users.Commands
{
    public class UserDelete : CommandMessageBase
    {
        public string UserId { get; set; } = string.Empty;
    }
}

