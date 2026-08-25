namespace CoConnect.Messaging.Users.Commands
{
    public class DeleteUser : CommandMessageBase
    {
        public string UserId { get; set; } = string.Empty;
    }
}

