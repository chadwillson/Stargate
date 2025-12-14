using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Events;
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

// Configure Serilog (skip in test environment to avoid "logger already frozen" error)
if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("Application", "Stargate.Api")
        .CreateBootstrapLogger();

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("Application", "Stargate.Api"));
}

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
builder.Services.AddScoped<ITestDataService, TestDataService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Register CorrelationId Accessor
builder.Services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

// Register Validators
builder.Services.AddValidatorsFromAssemblyContaining<PersonRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Configure JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "StargateSecretKeyForJWT2024-MinimumLength32Chars!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "StargateAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "StargateUI";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

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
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Force delete and recreate to ensure all tables exist
    logger.LogInformation("Recreating SQLite database...");
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    logger.LogInformation("Database created successfully");

    // Seed initial data
    logger.LogInformation("Seeding database...");
    DatabaseSeeder.Seed(context);
    logger.LogInformation("Database seeded successfully");
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCorrelationId();

// Use Serilog request logging only in non-test environments
if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        options.GetLevel = (httpContext, elapsed, ex) => ex != null
            ? LogEventLevel.Error
            : httpContext.Response.StatusCode > 499
                ? LogEventLevel.Error
                : httpContext.Response.StatusCode > 399
                    ? LogEventLevel.Warning
                    : LogEventLevel.Information;
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value);
            diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
            diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);

            // Capture CorrelationId from HttpContext.Items
            if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
            {
                diagnosticContext.Set("CorrelationId", correlationId);
            }
        };
    });
}

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    if (!builder.Environment.IsEnvironment("IntegrationTest"))
    {
        Log.Information("Starting Stargate API");
    }
    app.Run();
}
catch (Exception ex)
{
    if (!builder.Environment.IsEnvironment("IntegrationTest"))
    {
        Log.Fatal(ex, "Application terminated unexpectedly");
    }
    throw;
}
finally
{
    if (!builder.Environment.IsEnvironment("IntegrationTest"))
    {
        Log.CloseAndFlush();
    }
}

public partial class Program { }
