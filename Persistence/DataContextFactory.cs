using CrudDatastore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public interface IDataContextFactory
    {
        DataContext CreateDataContext();
    }

    public class DataContextFactory<TContext> : IDataContextFactory where TContext : UnitOfWorkBase
    {
        private readonly IDbContextFactory<TContext> _factory;

        public DataContextFactory(IDbContextFactory<TContext> factory)
        {
            _factory = factory;
        }

        public DataContext CreateDataContext()
        {
            return new DataContext(_factory.CreateDbContext());
        }
    }

    public class DataContext : DataContextBase
    {
        internal DataContext(UnitOfWorkBase unitOfWork)
            : base(unitOfWork)
        {}
    }
}
