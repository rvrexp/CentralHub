using CentralHub.Application;
using CentralHub.Application.Interfaces;
using CentralHub.Core.Domain.Entities;
using CentralHub.Infrastructure.Data;
using CentralHub.Infrastructure.Data.DbContext;
using CentralHub.Infrastructure.Data.Seeding;
using CentralHub.WebAPI.Middleware;
using CentralHub.WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// --- End Identity block ---

// --- JWT AUTHENTICATION ---
//
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});
//
// --- END JWT BLOCK ---

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- SEED THE DATABASE ROLES ---
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await RoleSeeder.SeedRolesAsync(services);
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while seeding the database roles.");
}
// --- END SEEDING ---


app.Run();
