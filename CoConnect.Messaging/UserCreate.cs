using CoConnect.Messaging.Abstractions;
using CoConnect.Persistence.Entities;

namespace CoConnect.Messaging
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

