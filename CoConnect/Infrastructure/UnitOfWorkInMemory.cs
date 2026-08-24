using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Entities;

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
            modelBuilder.Entity<User>();

            // Bootstrap Admin user for first-run access. Default password: Admin123
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = "bootstrap-admin",
                    Username = "Admin",
                    PasswordHash = "O2Esdae1BIpDX7bsgeUv+S1teVqLWpwXBw9qY8l6U7I=",
                    Role = UserRole.Admin,
                    IsDisabled = false,
                    SecurityStamp = "bootstrap-admin-stamp",
                    UpdatedUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
                }
            );
        }
    }
}
