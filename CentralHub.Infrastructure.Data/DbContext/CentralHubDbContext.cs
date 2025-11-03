using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using Microsoft.EntityFrameworkCore;

namespace CentralHub.Infrastructure.Data.DbContext
{
    public class CentralHubDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public CentralHubDbContext(DbContextOptions<CentralHubDbContext> options) : base(options) { }

        // Define DbSets for Aggregate Roots
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuration for Client Aggregate ---
            modelBuilder.Entity<Client>(client =>
            {
                client.ToTable("Clients");
                client.HasKey(c => c.Id);

                // Configure TenantId
                client.Property(c => c.TenantId).IsRequired();
                client.HasIndex(c => c.TenantId);

                // Configure basic properties
                client.Property(c => c.Name).IsRequired().HasMaxLength(100);
                client.Property(c => c.Email).IsRequired().HasMaxLength(150);
                client.Property(c => c.PhoneNumber).HasMaxLength(20);

                // *** Configure Encapsulated Collection ***
                var navigation = client.Metadata.FindNavigation(nameof(Client.Properties));
                navigation?.SetPropertyAccessMode(PropertyAccessMode.Field); // Use the private '_properties' field

                // *** Configure Owned Entities (Property and Address) ***
                client.OwnsMany(c => c.Properties, property =>
                {
                    property.ToTable("Properties");
                    property.WithOwner().HasForeignKey("ClientId"); // FK back to Client
                    property.HasKey(p => p.Id);

                    property.Property(p => p.Notes).HasMaxLength(500);

                    // Configure Address Value Object within Property
                    property.OwnsOne(p => p.Address, address =>
                    {
                        address.Property(a => a.Street).HasColumnName("Address_Street").IsRequired().HasMaxLength(100);
                        address.Property(a => a.City).HasColumnName("Address_City").IsRequired().HasMaxLength(50);
                        address.Property(a => a.State).HasColumnName("Address_State").IsRequired().HasMaxLength(50);
                        address.Property(a => a.ZipCode).HasColumnName("Address_ZipCode").IsRequired().HasMaxLength(10);
                    });
                });
            });
        }
    }
}
