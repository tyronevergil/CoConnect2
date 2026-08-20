using Persistence;
using Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoConnect.Infrastructure
{
    public class UnitOfWorkInMemory : UnitOfWorkBase
    {
        public UnitOfWorkInMemory(DbContextOptions options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>();

            //modelBuilder.Entity<Contact>().HasData(
            //        new Contact { ContactId = "1", Lastname = "Roson", Firstname = "Tyrone" }
            //    );
        }
    }
}
