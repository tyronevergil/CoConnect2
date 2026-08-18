using CrudDatastore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public interface IQueryContextFactory
    {
        QueryContext CreateQueryContext();
    }

    public class QueryContextFactory<TContext> : IQueryContextFactory where TContext : QueryUnitBase
    {
        private readonly IDbContextFactory<TContext> _factory;

        public QueryContextFactory(IDbContextFactory<TContext> factory)
        {
            _factory = factory;
        }

        public QueryContext CreateQueryContext()
        {
            var context = _factory.CreateDbContext();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return new QueryContext(context);
        }
    }
}
