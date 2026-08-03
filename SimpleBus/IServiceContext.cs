using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBus
{
    public interface IServiceContext
    {
        Task Publish<T>(T message) where T : IEventMessage;
    }
}
