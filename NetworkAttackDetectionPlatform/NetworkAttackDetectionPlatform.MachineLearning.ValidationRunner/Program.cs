using System.IO;
using Microsoft.ML;
using NetworkAttackDetectionPlatform.MachineLearning.Datasets;
using NetworkAttackDetectionPlatform.MachineLearning.Training;
using NetworkAttackDetectionPlatform.MachineLearning.Utilities;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            Console.WriteLine("???????????????????????????????????????????????????????????????");
            Console.WriteLine("  CICIDS2017 ML PIPELINE - PRODUCTION TRAINING");
            Console.WriteLine("???????????????????????????????????????????????????????????????");
            Console.WriteLine();

            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Usage: ValidationRunner <dataset_csv_path> <model_output_path>");
                Console.ResetColor();
                return 1;
            }

            string datasetPath = args[0];
            string modelOutputPath = args[1];

            // Validate dataset exists
            if (!File.Exists(datasetPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: Dataset file not found: {datasetPath}");
                Console.ResetColor();
                return 1;
            }

            // Create MLContext with seed 42
            var mlContext = new MLContext(seed: 42);

            // Create training pipeline dependencies
            var datasetLoader = new DatasetLoader();
            var modelSaver = new ModelSaver(mlContext);
            var trainingPipeline = new TrainingPipeline(datasetLoader, modelSaver, mlContext);

            // Create TrainingOptions
            var options = new TrainingOptions
            {
                DatasetPath = datasetPath,
                ModelOutputPath = modelOutputPath,
                NumberOfTrees = 100,
                TestSplit = 0.2f,
                DatasetName = "CICIDS2017",
                DatasetVersion = "1.0",
                EnableNormalization = true,
                ApplyLabelMapping = true,
                ValidateDataset = true,
                ValidationFailureThreshold = 0.1f
            };

            Console.WriteLine($"Dataset Path: {options.DatasetPath}");
            Console.WriteLine($"Model Output Path: {options.ModelOutputPath}");
            Console.WriteLine($"Number of Trees: {options.NumberOfTrees}");
            Console.WriteLine($"Test Split: {options.TestSplit}");
            Console.WriteLine($"Dataset: {options.DatasetName} v{options.DatasetVersion}");
            Console.WriteLine();

            // Execute training pipeline
            var result = await trainingPipeline.ExecuteAsync(options);

            if (result.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("? Training completed successfully");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("? Training failed");
                Console.ResetColor();
                return 1;
            }

            // Print training/validation sample counts
            Console.WriteLine();
            Console.WriteLine("Dataset Statistics:");
            Console.WriteLine($"  Training Samples: {result.TrainingSamplesCount}");
            Console.WriteLine($"  Validation Samples: {result.ValidationSamplesCount}");

            // Print available evaluation metrics
            Console.WriteLine();
            Console.WriteLine("Evaluation Metrics:");
            Console.WriteLine($"  Training Accuracy: {result.TrainingAccuracy:P2}");
            Console.WriteLine($"  Validation Accuracy: {result.ValidationAccuracy:P2}");
            Console.WriteLine($"  Precision: {result.Precision:P2}");
            Console.WriteLine($"  Recall: {result.Recall:P2}");
            Console.WriteLine($"  F1 Score: {result.F1Score:P2}");
            Console.WriteLine($"  Macro Accuracy: {result.MacroAccuracy:P2}");
            Console.WriteLine($"  Weighted F1: {result.WeightedF1:P2}");

            // Print model path and size
            if (!string.IsNullOrEmpty(result.ModelPath) && File.Exists(result.ModelPath))
            {
                Console.WriteLine();
                Console.WriteLine("Model Information:");
                Console.WriteLine($"  Model Path: {result.ModelPath}");
                Console.WriteLine($"  Model Size: {FormatBytes(result.ModelSizeBytes)}");
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"FATAL ERROR: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Console.ResetColor();
            return 1;
        }
    }

    static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
