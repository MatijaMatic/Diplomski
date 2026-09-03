using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using NetworkAttackDetectionPlatform.Infrastructure.Data;
using NetworkAttackDetectionPlatform.Domain.Interfaces;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using NetworkAttackDetectionPlatform.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using NetworkAttackDetectionPlatform.API.Middleware;
using System.Reflection;
using NetworkAttackDetectionPlatform.MachineLearning.Prediction;
using NetworkAttackDetectionPlatform.MachineLearning.Training;
using NetworkAttackDetectionPlatform.MachineLearning.Integration;
using NetworkAttackDetectionPlatform.MachineLearning.Interfaces;
using NetworkAttackDetectionPlatform.MachineLearning.Datasets;
using NetworkAttackDetectionPlatform.MachineLearning.Utilities;

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
builder.Services.AddScoped<IDashboardService, NetworkAttackDetectionPlatform.Application.Services.DashboardService>();

// Register Infrastructure repositories directly
builder.Services.AddScoped<IAttackDetectionRepository, AttackDetectionRepository>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();

// Register ML.NET context as singleton
builder.Services.AddSingleton<MLContext>(sp => new MLContext(seed: 42));

// Register MachineLearning services
// Keep deterministic PredictionService registered so the model-backed service can use it as a fallback
builder.Services.AddScoped<PredictionService>();
builder.Services.AddScoped<IPredictionService, ModelBackedPredictionService>();
builder.Services.AddScoped<IModelTrainer, RandomForestTrainer>();
builder.Services.AddScoped<IAttackPredictionService, AttackPredictionService>();
builder.Services.AddScoped<IModelManagementService, ModelManagementService>();
builder.Services.AddScoped<IDatasetLoader, DatasetLoader>();

// Register Training Pipeline services
builder.Services.AddScoped<ITrainingPipeline, TrainingPipeline>();
builder.Services.AddScoped<IModelLoader, ModelLoader>();
builder.Services.AddScoped<IModelSaver, ModelSaver>();

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
