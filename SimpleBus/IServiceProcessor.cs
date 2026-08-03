using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBus
{
    public interface IServiceProcessor
    {
        Task Process(IMessage message);
    }
}
