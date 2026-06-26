using System;
using Microsoft.EntityFrameworkCore;
using NetworkAttackDetectionPlatform.Infrastructure.Data;
using NetworkAttackDetectionPlatform.Domain.Interfaces;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using NetworkAttackDetectionPlatform.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Application services
builder.Services.AddScoped<IAttackDetectionService, NetworkAttackDetectionPlatform.Application.Services.AttackDetectionService>();
builder.Services.AddScoped<IRecommendationService, NetworkAttackDetectionPlatform.Application.Services.RecommendationService>();

// Register Infrastructure repositories directly
builder.Services.AddScoped<IAttackDetectionRepository, AttackDetectionRepository>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
