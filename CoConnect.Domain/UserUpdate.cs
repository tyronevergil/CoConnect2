using CoConnect.Domain.Abstractions;
using Persistence.Entities;

namespace CoConnect.Domain
{
    public class UserUpdate : CommandMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.User;

        public bool IsDisabled { get; set; }

        public string? Password { get; set; }
    }
}
