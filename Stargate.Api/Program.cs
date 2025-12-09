using Microsoft.EntityFrameworkCore;
using Stargate.Application.Interfaces;
using Stargate.Application.Services;
using Stargate.Repository;
using Stargate.Repository.Interfaces;
using Stargate.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Database
builder.Services.AddDbContext<StargateContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Application Services
builder.Services.AddScoped<IPersonAstronautService, PersonAstronautService>();
builder.Services.AddScoped<IAstronautDutyService, AstronautDutyService>();

builder.Services.AddControllers();
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
