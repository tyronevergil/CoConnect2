namespace CoConnect.Messaging.Users.Commands
{
    public class DisableUser : CommandMessageBase
    {
        public string UserId { get; set; } = string.Empty;
    }
}

