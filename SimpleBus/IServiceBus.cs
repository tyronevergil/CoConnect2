using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBus
{
    public interface IServiceBus
    {
        Task Send<T>(T message) where T : ICommandMessage;
    }
}
