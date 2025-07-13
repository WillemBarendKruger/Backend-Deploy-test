using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Potholio.Authorization.Roles;
using Potholio.Authorization.Users;
using Potholio.MultiTenancy;
using System;
using System.Linq;

namespace Potholio.EntityFrameworkCore
{
    public class PotholioDbContext : AbpZeroDbContext<Tenant, Role, User, PotholioDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public PotholioDbContext(DbContextOptions<PotholioDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),           // Convert to UTC when saving
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // Specify UTC kind when reading

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTime));

                foreach (var property in properties)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name)
                        .HasConversion(dateTimeConverter);
                }
            }
        }
    }
}
