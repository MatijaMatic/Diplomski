using System;
using Microsoft.EntityFrameworkCore;
using NetworkAttackDetectionPlatform.Infrastructure.Data;
using NetworkAttackDetectionPlatform.Domain.Interfaces;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using NetworkAttackDetectionPlatform.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using NetworkAttackDetectionPlatform.API.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

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

// Apply pending EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Applying pending migrations (if any)...");
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying database migrations.");
        // Re-throw to prevent app from starting in inconsistent state
        throw;
    }
}

// Configure the HTTP request pipeline.
// Register exception handling middleware early to catch errors from downstream
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
