using System.Threading.Tasks;
using CoConnect.Domain;
using Microsoft.AspNetCore.SignalR;

namespace CoConnect.Infrastructure
{
    public class SignalRNotificationDispatcher : INotificationDispatcher
    {
        private readonly IHubContext<MessageHub> _hubContext;

        public SignalRNotificationDispatcher(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PublishAsync<T>(string method, T message)
        {
            await _hubContext.Clients.All.SendAsync(method, message);
        }

        public async Task PublishAsync<T>(string connectionId, string method, T message)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync(method, message);
        }
    }
}
