using CrudDatastore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoConnect.Persistence
{
    public class DataContext : DataContextBase
    {
        internal DataContext(UnitOfWorkBase unitOfWork)
            : base(unitOfWork)
        { }
    }
}

