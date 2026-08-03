using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBus
{
    public interface IMessageHandler
    {
    }

    public interface IMessageHandler<T> : IMessageHandler where T : IMessage
    {
        Task Handle(IServiceContext context, T message);
    }
}
