using CoConnect.Domain.Abstractions;

namespace CoConnect.Domain
{
    public class UserUpdated : EventMessageBase
    {
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public bool RequiresSessionRefresh { get; set; }
    }
}
