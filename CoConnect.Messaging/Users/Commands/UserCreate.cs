using CoConnect.Persistence.Entities;

namespace CoConnect.Messaging.Users.Commands
{
    public class UserCreate : CommandMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.User;

        public bool IsDisabled { get; set; }
    }
}

