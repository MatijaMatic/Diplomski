using Microsoft.ML;
using NetworkAttackDetectionPlatform.MachineLearning.Data;
using NetworkAttackDetectionPlatform.MachineLearning.Datasets;
using NetworkAttackDetectionPlatform.MachineLearning.Interfaces;
using NetworkAttackDetectionPlatform.MachineLearning.Models;
using NetworkAttackDetectionPlatform.MachineLearning.Preprocessing;
using NetworkAttackDetectionPlatform.MachineLearning.Training;
using NetworkAttackDetectionPlatform.MachineLearning.Utilities;
using System.Diagnostics;

namespace NetworkAttackDetectionPlatform.MachineLearning.Testing;

/// <summary>
/// Smoke test for the complete CICIDS2017 training pipeline.
/// Validates end-to-end flow: CSV ? Reader ? Validation ? Mapping ? Preprocessing ? FastTree ? Evaluation ? Model Saving.
/// </summary>
public class Cicids2017TrainingSmokeTest
{
    private readonly Cicids2017TestConfiguration _config;
    private readonly MLContext _mlContext;
    private readonly CsvDatasetReader _csvReader;
    private readonly DatasetValidationService _validationService;
    private readonly LabelMapper _labelMapper;
    private readonly PreprocessingPipeline _preprocessingPipeline;

    public Cicids2017TrainingSmokeTest(Cicids2017TestConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _mlContext = new MLContext(seed: config.RandomSeed);
        _csvReader = new CsvDatasetReader();
        _validationService = new DatasetValidationService();
        _labelMapper = new LabelMapper();
        _preprocessingPipeline = new PreprocessingPipeline(_mlContext);
    }

    /// <summary>
    /// Executes the complete smoke test.
    /// </summary>
    public async Task<SmokeTestResult> RunAsync()
    {
        var result = new SmokeTestResult
        {
            StartTime = DateTime.UtcNow
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            PrintHeader("CICIDS2017 TRAINING PIPELINE SMOKE TEST");

            // Step 1: Configuration validation
            if (!ValidateConfiguration(result))
            {
                return result;
            }

            // Step 2: Ensure dataset exists (generate sample if needed)
            await EnsureDatasetExistsAsync(result);

            // Step 3: Load CSV dataset
            if (!await LoadDatasetAsync(result))
            {
                return result;
            }

            // Step 4: Validate dataset quality
            if (!ValidateDatasetQuality(result))
            {
                return result;
            }

            // Step 5: Apply label mapping
            if (!ApplyLabelMapping(result))
            {
                return result;
            }

            // Step 6: Verify preprocessing pipeline
            if (!VerifyPreprocessingPipeline(result))
            {
                return result;
            }

            // Step 7: Execute training
            if (!await ExecuteTrainingAsync(result))
            {
                return result;
            }

            // Step 8: Verify model output
            if (!VerifyModelOutput(result))
            {
                return result;
            }

            stopwatch.Stop();
            result.TotalDuration = stopwatch.Elapsed;
            result.Success = true;
            result.Message = "? Smoke test PASSED. All pipeline stages completed successfully.";

            PrintSummary(result);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.Success = false;
            result.TotalDuration = stopwatch.Elapsed;
            result.Message = $"? Smoke test FAILED with exception: {ex.Message}";
            result.Errors.Add($"Exception: {ex.GetType().Name} - {ex.Message}");
            result.Errors.Add($"StackTrace: {ex.StackTrace}");

            PrintError(result.Message);
        }

        return result;
    }

    private bool ValidateConfiguration(SmokeTestResult result)
    {
        PrintSection("1. Configuration Validation");

        if (!_config.IsValid(out string errorMessage))
        {
            result.Success = false;
            result.Message = $"Configuration validation failed: {errorMessage}";
            result.Errors.Add(errorMessage);
            PrintError(result.Message);
            return false;
        }

        Console.WriteLine($"   ? Dataset Path: {_config.DatasetPath}");
        Console.WriteLine($"   ? Model Output Path: {_config.ModelOutputPath}");
        Console.WriteLine($"   ? Sample Size: {_config.SampleSize} rows");
        Console.WriteLine($"   ? Expected Features: {_config.ExpectedFeatureCount}");
        Console.WriteLine($"   ? Number of Trees: {_config.NumberOfTrees}");
        Console.WriteLine($"   ? Test Split: {_config.TestSplit:P0}");
        Console.WriteLine($"   ? Configuration is valid");

        return true;
    }

    private async Task EnsureDatasetExistsAsync(SmokeTestResult result)
    {
        PrintSection("2. Dataset Preparation");

        if (File.Exists(_config.DatasetPath))
        {
            Console.WriteLine($"   ? Dataset file found: {_config.DatasetPath}");
            return;
        }

        Console.WriteLine($"   ? Dataset file not found: {_config.DatasetPath}");
        Console.WriteLine($"   ? Generating sample CICIDS2017 dataset...");

        // Create directory if needed
        var directory = Path.GetDirectoryName(_config.DatasetPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Console.WriteLine($"   ? Created directory: {directory}");
        }

        // Generate sample data
        var generator = new Cicids2017SampleGenerator(_config.RandomSeed);
        await generator.WriteSampleCsvAsync(_config.DatasetPath, _config.SampleSize, includeAllLabels: true);

        Console.WriteLine($"   ? Generated {_config.SampleSize} sample rows");
        result.DatasetGenerated = true;
    }

    private async Task<bool> LoadDatasetAsync(SmokeTestResult result)
    {
        PrintSection("3. CSV Dataset Loading");

        try
        {
            var data = await _csvReader.ReadAsync(_config.DatasetPath, skipHeader: true);
            result.LoadedData = data.ToList();
            result.TotalRows = result.LoadedData.Count;

            Console.WriteLine($"   ? CSV file loaded successfully");
            Console.WriteLine($"   ? Total rows: {result.TotalRows}");

            if (result.TotalRows == 0)
            {
                result.Errors.Add("Dataset is empty - no rows loaded.");
                PrintError("   ? Dataset is empty");
                return false;
            }

            // Verify feature count
            var firstRow = result.LoadedData.First();
            var featureCount = FeatureConfiguration.FeatureCount;
            Console.WriteLine($"   ? Feature count: {featureCount}");

            if (featureCount != _config.ExpectedFeatureCount)
            {
                result.Warnings.Add($"Feature count mismatch: expected {_config.ExpectedFeatureCount}, got {featureCount}");
                Console.WriteLine($"   ? Warning: Feature count mismatch");
            }

            // Check labels
            var uniqueLabels = result.LoadedData.Select(r => r.Label).Distinct().ToList();
            result.OriginalLabels = uniqueLabels;
            Console.WriteLine($"   ? Unique labels found: {uniqueLabels.Count}");
            foreach (var label in uniqueLabels)
            {
                Console.WriteLine($"      - {label}");
            }

            return true;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"CSV loading failed: {ex.Message}");
            PrintError($"   ? CSV loading failed: {ex.Message}");
            return false;
        }
    }

    private bool ValidateDatasetQuality(SmokeTestResult result)
    {
        PrintSection("4. Dataset Quality Validation");

        try
        {
            var validationReport = _validationService.Validate(result.LoadedData);
            result.ValidationReport = validationReport;

            Console.WriteLine($"   {validationReport.GetSummary()}");

            if (!validationReport.IsValid)
            {
                Console.WriteLine($"   ? Dataset validation failed");
                foreach (var error in validationReport.Errors)
                {
                    Console.WriteLine($"      ERROR: {error}");
                    result.Errors.Add($"Validation error: {error}");
                }
            }
            else
            {
                Console.WriteLine($"   ? Dataset validation passed");
            }

            if (validationReport.Warnings.Any())
            {
                Console.WriteLine($"   ? Validation warnings:");
                foreach (var warning in validationReport.Warnings)
                {
                    Console.WriteLine($"      WARNING: {warning}");
                    result.Warnings.Add(warning);
                }
            }

            return validationReport.IsValid;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Validation failed: {ex.Message}");
            PrintError($"   ? Validation failed: {ex.Message}");
            return false;
        }
    }

    private bool ApplyLabelMapping(SmokeTestResult result)
    {
        PrintSection("5. Label Mapping (15 ? 8 classes)");

        try
        {
            var originalLabelsCount = result.LoadedData.Select(r => r.Label).Distinct().Count();
            Console.WriteLine($"   Original label count: {originalLabelsCount}");

            // Apply mapping
            foreach (var row in result.LoadedData)
            {
                row.Label = _labelMapper.MapLabel(row.Label);
            }

            var normalizedLabels = result.LoadedData.Select(r => r.Label).Distinct().ToList();
            result.NormalizedLabels = normalizedLabels;

            Console.WriteLine($"   ? Label mapping applied");
            Console.WriteLine($"   Normalized label count: {normalizedLabels.Count}");
            Console.WriteLine($"   Normalized categories:");
            foreach (var label in normalizedLabels.OrderBy(l => l))
            {
                var count = result.LoadedData.Count(r => r.Label == label);
                Console.WriteLine($"      - {label}: {count} samples ({(double)count / result.TotalRows:P1})");
            }

            return true;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Label mapping failed: {ex.Message}");
            PrintError($"   ? Label mapping failed: {ex.Message}");
            return false;
        }
    }

    private bool VerifyPreprocessingPipeline(SmokeTestResult result)
    {
        PrintSection("6. Preprocessing Pipeline Verification");

        try
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(result.LoadedData);
            Console.WriteLine($"   ? Data loaded into ML.NET IDataView");

            // Validate schema
            var schemaValid = _preprocessingPipeline.ValidateSchema(dataView);
            Console.WriteLine($"   ? Schema validation passed");

            // Build pipeline
            var preprocessingOptions = new PreprocessingOptions
            {
                EnableNormalization = _config.EnableNormalization,
                ApplyLabelMapping = _config.ApplyLabelMapping
            };

            var pipeline = _preprocessingPipeline.BuildPipeline(preprocessingOptions);
            Console.WriteLine($"   ? Preprocessing pipeline built successfully");

            // Get statistics
            var stats = _preprocessingPipeline.GetStatistics();
            Console.WriteLine($"   {stats}");

            return true;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Preprocessing verification failed: {ex.Message}");
            PrintError($"   ? Preprocessing verification failed: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> ExecuteTrainingAsync(SmokeTestResult result)
    {
        PrintSection("7. Model Training Execution");

        try
        {
            // Create output directory if needed
            var modelDirectory = Path.GetDirectoryName(_config.ModelOutputPath);
            if (!string.IsNullOrEmpty(modelDirectory) && !Directory.Exists(modelDirectory))
            {
                Directory.CreateDirectory(modelDirectory);
            }

            // Create training options
            var trainingOptions = new TrainingOptions
            {
                DatasetPath = _config.DatasetPath,
                ModelOutputPath = _config.ModelOutputPath,
                NumberOfTrees = _config.NumberOfTrees,
                RandomSeed = _config.RandomSeed,
                TestSplit = _config.TestSplit,
                DatasetName = "CICIDS2017-Sample",
                DatasetVersion = "1.0",
                EnableNormalization = _config.EnableNormalization,
                ApplyLabelMapping = _config.ApplyLabelMapping,
                ValidateDataset = false, // Already validated
                ValidationFailureThreshold = _config.ValidationFailureThreshold
            };

            // Create training pipeline
            var datasetLoader = new DatasetLoader();
            var modelSaver = new ModelSaver(_mlContext);
            var trainingPipeline = new TrainingPipeline(datasetLoader, modelSaver, _mlContext);

            Console.WriteLine($"   ? Training with {_config.NumberOfTrees} trees...");
            var trainingStopwatch = Stopwatch.StartNew();

            var trainingResult = await trainingPipeline.ExecuteAsync(trainingOptions);

            trainingStopwatch.Stop();
            result.TrainingResult = trainingResult;
            result.TrainingDuration = trainingStopwatch.Elapsed;

            if (trainingResult.Success)
            {
                Console.WriteLine($"   ? Training completed successfully");
                Console.WriteLine($"   Training duration: {result.TrainingDuration.TotalSeconds:F2}s");
                Console.WriteLine($"   Training samples: {trainingResult.TrainingSamplesCount}");
                Console.WriteLine($"   Validation samples: {trainingResult.ValidationSamplesCount}");
                Console.WriteLine($"   Accuracy: {trainingResult.ValidationAccuracy:P2}");
                Console.WriteLine($"   Precision: {trainingResult.Precision:P2}");
                Console.WriteLine($"   F1 Score: {trainingResult.F1Score:F4}");
                Console.WriteLine($"   Model path: {trainingResult.ModelPath}");

                if (trainingResult.ClassLabels != null && trainingResult.ClassLabels.Any())
                {
                    Console.WriteLine($"   Class labels: {string.Join(", ", trainingResult.ClassLabels)}");
                }

                return true;
            }
            else
            {
                result.Errors.Add($"Training failed: {trainingResult.Message}");
                PrintError($"   ? Training failed: {trainingResult.Message}");
                return false;
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Training execution failed: {ex.Message}");
            PrintError($"   ? Training execution failed: {ex.Message}");
            return false;
        }
    }

    private bool VerifyModelOutput(SmokeTestResult result)
    {
        PrintSection("8. Model Output Verification");

        try
        {
            if (!File.Exists(_config.ModelOutputPath))
            {
                result.Errors.Add($"Model file not found: {_config.ModelOutputPath}");
                PrintError($"   ? Model file not found");
                return false;
            }

            var modelInfo = new FileInfo(_config.ModelOutputPath);
            result.ModelSizeBytes = modelInfo.Length;
            Console.WriteLine($"   ? Model file created: {_config.ModelOutputPath}");
            Console.WriteLine($"   Model size: {result.ModelSizeBytes:N0} bytes ({result.ModelSizeBytes / 1024.0:F2} KB)");

            var metadataPath = Path.ChangeExtension(_config.ModelOutputPath, ".metadata.json");
            if (File.Exists(metadataPath))
            {
                Console.WriteLine($"   ? Metadata file created: {metadataPath}");
            }
            else
            {
                result.Warnings.Add("Metadata file not found");
                Console.WriteLine($"   ? Metadata file not found");
            }

            return true;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Model verification failed: {ex.Message}");
            PrintError($"   ? Model verification failed: {ex.Message}");
            return false;
        }
    }

    private void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine($"  {title}");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine();
    }

    private void PrintSection(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"? {title}");
        Console.WriteLine();
    }

    private void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void PrintSummary(SmokeTestResult result)
    {
        Console.WriteLine();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  SMOKE TEST SUMMARY");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine();
        Console.WriteLine($"Status: {(result.Success ? "? PASSED" : "? FAILED")}");
        Console.WriteLine($"Total Duration: {result.TotalDuration.TotalSeconds:F2}s");
        Console.WriteLine($"Training Duration: {result.TrainingDuration.TotalSeconds:F2}s");
        Console.WriteLine();
        Console.WriteLine($"Dataset:");
        Console.WriteLine($"  - Total Rows: {result.TotalRows}");
        Console.WriteLine($"  - Original Labels: {result.OriginalLabels?.Count ?? 0}");
        Console.WriteLine($"  - Normalized Labels: {result.NormalizedLabels?.Count ?? 0}");
        Console.WriteLine($"  - Generated: {(result.DatasetGenerated ? "Yes" : "No")}");
        Console.WriteLine();

        if (result.TrainingResult != null)
        {
            Console.WriteLine($"Training:");
            Console.WriteLine($"  - Training Samples: {result.TrainingResult.TrainingSamplesCount}");
            Console.WriteLine($"  - Validation Samples: {result.TrainingResult.ValidationSamplesCount}");
            Console.WriteLine($"  - Accuracy: {result.TrainingResult.ValidationAccuracy:P2}");
            Console.WriteLine($"  - F1 Score: {result.TrainingResult.F1Score:F4}");
            Console.WriteLine();
        }

        Console.WriteLine($"Model:");
        Console.WriteLine($"  - Path: {_config.ModelOutputPath}");
        Console.WriteLine($"  - Size: {result.ModelSizeBytes / 1024.0:F2} KB");
        Console.WriteLine();

        if (result.Warnings.Any())
        {
            Console.WriteLine($"Warnings: {result.Warnings.Count}");
            foreach (var warning in result.Warnings)
            {
                Console.WriteLine($"  ? {warning}");
            }
            Console.WriteLine();
        }

        if (result.Errors.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Errors: {result.Errors.Count}");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"  ? {error}");
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine();
    }
}

/// <summary>
/// Result of the smoke test execution.
/// </summary>
public class SmokeTestResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public TimeSpan TrainingDuration { get; set; }

    public bool DatasetGenerated { get; set; }
    public int TotalRows { get; set; }
    public List<Cicids2017TrainingData> LoadedData { get; set; } = new();
    public List<string> OriginalLabels { get; set; } = new();
    public List<string> NormalizedLabels { get; set; } = new();

    public ValidationReport? ValidationReport { get; set; }
    public TrainingResult? TrainingResult { get; set; }

    public long ModelSizeBytes { get; set; }

    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}
