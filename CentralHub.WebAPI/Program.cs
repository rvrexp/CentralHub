using CentralHub.Application;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Entities;
using CentralHub.Infrastructure.Data;
using CentralHub.Infrastructure.Data.DbContext;
using CentralHub.WebAPI.Middleware;
using CentralHub.WebAPI.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Call extension methods to register services from other layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureDataServices(builder.Configuration);
// builder.Services.AddInfrastructureServices(builder.Configuration); // Add later
builder.Services.AddControllers();
// Register WebAPI specific services
builder.Services.AddHttpContextAccessor(); // Needed by CurrentUserService
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>(); // Register placeholder service

// ---Identity ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<CentralHubDbContext>() // Tell Identity to use DbContext
.AddDefaultTokenProviders(); // Adds services for things like password reset tokens
//
// --- End Identity block ---


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

//Custom middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
