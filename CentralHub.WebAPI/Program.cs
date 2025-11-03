using CentralHub.Application;
using CentralHub.Application.Interfaces;
using CentralHub.Infrastructure.Data;
using CentralHub.WebAPI.Services;

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
