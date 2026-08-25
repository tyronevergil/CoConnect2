using CrudDatastore;
using CrudDatastore.Foundation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoConnect.Persistence
{
    public abstract class QueryUnitBase : DbContext, IQueryUnitSync
    {
        protected QueryUnitBase(DbContextOptions options)
            : base(options)
        {
        }

        private readonly IDictionary<Type, object> _dataQueries = new Dictionary<Type, object>();

        public event EventHandler<EntityEventArgs> EntityMaterialized;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .AddInterceptors(new MaterializationInterceptor((entity) =>
                {
                    RaiseEntityMaterialized(entity);
                    return entity;
                }));
        }

        protected void RaiseEntityMaterialized(object entity)
        {
            EntityMaterialized?.Invoke(this, new EntityEventArgs(entity));
        }

        public void Execute(string command, params object[] parameters)
        {
            //Database.ExecuteSqlRaw(command, parameters);
        }

        public IDataQuery<T> Read<T>() where T : EntityBase
        {
            var entityType = typeof(T);
            if (_dataQueries.ContainsKey(entityType))
                return (IDataQuery<T>)_dataQueries[entityType];

            var dataQuery = new DataQuery<T>(new DbSetQueryAdapter<T>(this));
            _dataQueries.Add(entityType, dataQuery);

            return dataQuery;
        }
    }
}

