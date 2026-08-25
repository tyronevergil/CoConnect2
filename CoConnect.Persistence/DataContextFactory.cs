using CrudDatastore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoConnect.Persistence
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
            var context = _factory.CreateDbContext();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return new DataContext(context);
        }
    }
}

