using CrudDatastore;
using CrudDatastore.Foundation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
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
