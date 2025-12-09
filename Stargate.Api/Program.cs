using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Stargate.Application.Interfaces;
using Stargate.Application.Services;
using Stargate.Application.Validators;
using Stargate.Repository;
using Stargate.Repository.Interfaces;
using Stargate.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Database
var useInMemory = builder.Environment.IsEnvironment("IntegrationTest");
builder.Services.AddDbContext<StargateContext>(options =>
{
    if (useInMemory)
    {
        options.UseInMemoryDatabase("IntegrationTestsDb");
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Application Services
builder.Services.AddScoped<IPersonAstronautService, PersonAstronautService>();
builder.Services.AddScoped<IAstronautDutyService, AstronautDutyService>();

// Register Validators
builder.Services.AddValidatorsFromAssemblyContaining<PersonRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
