using CentralHub.Application.Interfaces;
using CentralHub.Infrastructure.Data.DbContext;
using CentralHub.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CentralHub.Infrastructure.Data
{
    // Extension method for registering Infrastructure.Data services (DI setup)
    public static class InfrastructureDataRegistration
    {
        public static IServiceCollection AddInfrastructureDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<CentralHubDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); // Reads connection string

            // Register repositories and Unit of Work (Scoped lifetime is standard)
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
