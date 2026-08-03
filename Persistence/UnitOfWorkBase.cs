using CrudDatastore;
using CrudDatastore.Foundation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public abstract class UnitOfWorkBase : DbContext, IUnitOfWorkSync
    {
        public UnitOfWorkBase(DbContextOptions options)
            : base(options)
        {
        }

        private readonly IDictionary<Type, object> _dataQueries = new Dictionary<Type, object>();

        public event EventHandler<EntityEventArgs> EntityMaterialized;
        public event EventHandler<EntityEventArgs> EntityCreate;
        public event EventHandler<EntityEventArgs> EntityUpdate;
        public event EventHandler<EntityEventArgs> EntityDelete;

        public override int SaveChanges()
        {
            OnBeforeSaveChanges();
            return base.SaveChanges();
        }

        private void OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();

            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        EntityCreate?.Invoke(this, new EntityEventArgs(entry.Entity));
                        break;
                    case EntityState.Modified:
                        EntityUpdate?.Invoke(this, new EntityEventArgs(entry.Entity));
                        break;
                    case EntityState.Deleted:
                        EntityDelete?.Invoke(this, new EntityEventArgs(entry.Entity));
                        break;
                }
            }
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

        public void MarkNew<T>(T entity) where T : EntityBase
        {
            Set<T>().Add(entity);
        }

        public void MarkModified<T>(T entity) where T : EntityBase
        {
            var entry = Entry(entity);
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                entry.State = EntityState.Modified;
        }

        public void MarkDeleted<T>(T entity) where T : EntityBase
        {
            var entry = Entry(entity);
            if (entry.State == EntityState.Detached)
                Set<T>().Attach(entity);

            Set<T>().Remove(entity);
        }

        public void Commit()
        {
            SaveChanges();
        }
    }

    internal class DbSetQueryAdapter<T> : DelegateQueryAdapter<T> where T : EntityBase
    {
        public DbSetQueryAdapter(DbContext dbContext)
            : base
            (
                /* read */
                (predicate) =>
                {
                    return dbContext.Set<T>().Where(predicate);
                },

                /* read - command */
                (command, parameters) =>
                {
                    //return dbContext.Set<T>().FromSqlRaw(command, parameters).AsQueryable();
                    return Enumerable.Empty<T>().AsQueryable();
                }
            )
        { }
    }
}
