using CentralHub.Core.Domain.Aggregates.ClientAggregate;
using CentralHub.Core.Domain.Aggregates.JobAggregate;
using CentralHub.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CentralHub.Infrastructure.Data.DbContext
{
    public class CentralHubDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public CentralHubDbContext(DbContextOptions<CentralHubDbContext> options) : base(options) { }

        // Define DbSets for Aggregate Roots
        public DbSet<Client> Clients { get; set; }
        public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Tells EF Core to build all the
            // Identity tables (AspNetUsers, AspNetRoles, etc.).
            base.OnModelCreating(modelBuilder);

            // Configuration for Client Aggregate 
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

            // --- Configuration for Job Aggregate ---
            modelBuilder.Entity<Job>(job =>
            {
                job.ToTable("Jobs");
                job.HasKey(j => j.Id);

                // Configure TenantId
                job.Property(j => j.TenantId).IsRequired();
                job.HasIndex(j => j.TenantId);

                // Configure properties
                job.Property(j => j.Status).IsRequired();
                job.Property(j => j.ScheduledStartTime).IsRequired();
                job.Property(j => j.Notes).HasMaxLength(1000);

                // Configure relationships to Client and Property
                // A Job belongs to one Client. A Client can have many Jobs.
                job.HasOne(j => j.Client)
                    .WithMany() // A Client doesn't need a List<Job>
                    .HasForeignKey(j => j.ClientId)
                    .OnDelete(DeleteBehavior.Restrict); // Don't delete a Client if they have Jobs

                // A Job belongs to one Property. A Property can have many Jobs.
                job.HasOne(j => j.Property)
                    .WithMany() // A Property doesn't need a List<Job>
                    .HasForeignKey(j => j.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict); // Don't delete a Property if it has Jobs
            });


            // (Optional but good practice) Customize Identity table names if you don't like "AspNet..."
            modelBuilder.Entity<ApplicationUser>(b => b.ToTable("Users"));
            modelBuilder.Entity<IdentityRole<Guid>>(b => b.ToTable("Roles"));
            modelBuilder.Entity<IdentityUserRole<Guid>>(b => b.ToTable("UserRoles"));
            modelBuilder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable("UserClaims"));
            modelBuilder.Entity<IdentityUserLogin<Guid>>(b => b.ToTable("UserLogins"));
            modelBuilder.Entity<IdentityRoleClaim<Guid>>(b => b.ToTable("RoleClaims"));
            modelBuilder.Entity<IdentityUserToken<Guid>>(b => b.ToTable("UserTokens"));
        }
    }
}
