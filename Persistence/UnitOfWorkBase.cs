using CrudDatastore;
using CrudDatastore.Foundation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static Persistence.QueryUnitBase;

namespace Persistence
{
    public abstract class UnitOfWorkBase : QueryUnitBase, IUnitOfWorkSync
    {
        public UnitOfWorkBase(DbContextOptions options)
            : base(options)
        {
        }

        public event EventHandler<EntityEventArgs> EntityCreate;
        public event EventHandler<EntityEventArgs> EntityUpdate;
        public event EventHandler<EntityEventArgs> EntityDelete;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

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
}
