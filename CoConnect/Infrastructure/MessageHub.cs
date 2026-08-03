using Microsoft.AspNetCore.SignalR;

namespace CoConnect.Infrastructure
{
    public class MessageHub : Hub
    {
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
