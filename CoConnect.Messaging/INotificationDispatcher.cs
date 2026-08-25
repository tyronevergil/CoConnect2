using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoConnect.Messaging
{
    public interface INotificationDispatcher
    {
        Task PublishAsync<T>(string method, T message);
        Task PublishAsync<T>(string connectionId, string method, T message);
    }
}

