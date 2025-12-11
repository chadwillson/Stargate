using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Stargate.Api.Middleware;
using Stargate.Application.Interfaces;
using Stargate.Application.Services;
using Stargate.Application.Validators;
using Stargate.Domain.Interfaces;
using Stargate.Domain.Services;
using Stargate.Repository;
using Stargate.Repository.Interfaces;
using Stargate.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Database
var useInMemory = builder.Environment.IsEnvironment("IntegrationTest");
var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

builder.Services.AddDbContext<StargateContext>(options =>
{
    if (useInMemory)
    {
        options.UseInMemoryDatabase("IntegrationTestsDb");
    }
    else if (databaseProvider == "Sqlite")
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Repositories (needed by domain services) - Get from UnitOfWork to ensure same context
builder.Services.AddScoped<IPersonAstronautRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().PersonAstronauts);
builder.Services.AddScoped<IAstronautDetailRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().AstronautDetails);
builder.Services.AddScoped<IAstronautDutyRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().AstronautDuties);
builder.Services.AddScoped<ILogRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().LogEntries);

// Register Domain Services
builder.Services.AddScoped<IPersonDomainService, PersonDomainService>();
builder.Services.AddScoped<IAstronautDutyDomainService, AstronautDutyDomainService>();

// Register Application Services
builder.Services.AddScoped<IPersonAstronautService, PersonAstronautService>();
builder.Services.AddScoped<IAstronautDutyService, AstronautDutyService>();
builder.Services.AddScoped<ILoggingService, DatabaseLoggingService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

// Register CorrelationId Accessor
builder.Services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

// Register Validators
builder.Services.AddValidatorsFromAssemblyContaining<PersonRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:61503")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Initialize database for development with SQLite
if (builder.Environment.IsDevelopment() && databaseProvider == "Sqlite")
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<StargateContext>();

    // Ensure database is created
    context.Database.EnsureCreated();

    // Seed initial data
    DatabaseSeeder.Seed(context);
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCorrelationId();

app.UseCors("AllowAngularApp");

app.UseTokenAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
