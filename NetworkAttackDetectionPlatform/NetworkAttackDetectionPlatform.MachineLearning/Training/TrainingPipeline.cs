using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using NetworkAttackDetectionPlatform.MachineLearning.Data;
using NetworkAttackDetectionPlatform.MachineLearning.Datasets;
using NetworkAttackDetectionPlatform.MachineLearning.Evaluation;
using NetworkAttackDetectionPlatform.MachineLearning.Interfaces;
using NetworkAttackDetectionPlatform.MachineLearning.Models;
using NetworkAttackDetectionPlatform.MachineLearning.Preprocessing;

namespace NetworkAttackDetectionPlatform.MachineLearning.Training
{
    /// <summary>
    /// Orchestrates the complete ML training pipeline.
    /// Responsible for: loading dataset, preprocessing, train/test split, training, evaluation, and model persistence.
    /// </summary>
    public class TrainingPipeline : ITrainingPipeline
    {
        private readonly IDatasetLoader _datasetLoader;
        private readonly IModelSaver _modelSaver;
        private readonly MLContext _mlContext;
        private readonly CsvDatasetReader _csvReader;
        private readonly DatasetValidationService _validationService;
        private readonly LabelMapper _labelMapper;
        private readonly PreprocessingPipeline _preprocessingPipeline;

        public TrainingPipeline(
            IDatasetLoader datasetLoader,
            IModelSaver modelSaver,
            MLContext mlContext)
        {
            _datasetLoader = datasetLoader ?? throw new ArgumentNullException(nameof(datasetLoader));
            _modelSaver = modelSaver ?? throw new ArgumentNullException(nameof(modelSaver));
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));

            // Initialize CICIDS2017-specific components
            _csvReader = new CsvDatasetReader();
            _validationService = new DatasetValidationService();
            _labelMapper = new LabelMapper();
            _preprocessingPipeline = new PreprocessingPipeline(_mlContext);
        }

        /// <summary>
        /// Executes the complete training pipeline.
        /// </summary>
        public async Task<TrainingResult> ExecuteAsync(TrainingOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.DatasetPath))
                throw new ArgumentException("Dataset path must be specified in training options.", nameof(options));

            var stopwatch = Stopwatch.StartNew();
            var result = new TrainingResult
            {
                TrainedAt = DateTime.UtcNow
            };

            try
            {
                // Step 1: Load CICIDS2017 dataset from CSV
                Console.WriteLine($"Loading CICIDS2017 dataset from: {options.DatasetPath}");
                var cicidsData = await _csvReader.ReadAsync(options.DatasetPath, skipHeader: true).ConfigureAwait(false);
                var dataList = cicidsData.ToList();
                Console.WriteLine($"Loaded {dataList.Count} rows from CSV.");

                // Step 2: Validate dataset quality
                if (options.ValidateDataset)
                {
                    Console.WriteLine("Validating dataset...");
                    var validationReport = _validationService.Validate(dataList);
                    Console.WriteLine(validationReport.GetSummary());

                    if (!validationReport.IsValid)
                    {
                        result.Success = false;
                        result.Message = $"Dataset validation failed: {string.Join("; ", validationReport.Errors)}";
                        return result;
                    }

                    if (validationReport.InvalidRows > dataList.Count * options.ValidationFailureThreshold)
                    {
                        result.Success = false;
                        result.Message = $"Too many invalid rows: {validationReport.InvalidRows}/{dataList.Count} ({(double)validationReport.InvalidRows / dataList.Count:P2})";
                        return result;
                    }
                }

                // Step 3: Apply label mapping (15 ? 8 classes)
                if (options.ApplyLabelMapping)
                {
                    Console.WriteLine("Applying label mapping (15 ? 8 normalized categories)...");
                    foreach (var row in dataList)
                    {
                        row.Label = _labelMapper.MapLabel(row.Label);
                    }
                    Console.WriteLine($"Label mapping complete. Categories: {string.Join(", ", _labelMapper.GetNormalizedCategories())}");
                }

                // Step 4: Load data into ML.NET DataView
                var dataView = _mlContext.Data.LoadFromEnumerable(dataList);
                Console.WriteLine($"Loaded {dataList.Count} samples into ML.NET DataView.");

                // Step 5: Split into training and validation sets
                var (trainSet, testSet) = SplitData(dataView, options.TestSplit, options.RandomSeed);
                result.TrainingSamplesCount = (int)(trainSet.GetRowCount() ?? 0);
                result.ValidationSamplesCount = (int)(testSet.GetRowCount() ?? 0);
                Console.WriteLine($"Train/Test split: {result.TrainingSamplesCount}/{result.ValidationSamplesCount}");

                // Step 6: Build preprocessing and training pipeline
                var trainingPipeline = BuildTrainingPipeline(options);

                // Step 7: Train the model
                var trainedModel = TrainModel(trainingPipeline, trainSet);

                // Step 8: Evaluate on validation set
                var metrics = EvaluateModel(trainedModel, testSet);

                // Extract correct metrics from confusion matrix
                var classLabels = ExtractClassLabels();
                var perClassMetrics = EvaluationMetricsExtractor.ExtractPerClassMetrics(metrics.ConfusionMatrix, classLabels);
                var aggregateMetrics = EvaluationMetricsExtractor.CalculateAggregateMetrics(perClassMetrics, metrics.MicroAccuracy, metrics.MacroAccuracy);

                result.ValidationAccuracy = aggregateMetrics.MicroAccuracy;
                result.Precision = aggregateMetrics.MacroPrecision;
                result.Recall = aggregateMetrics.MacroRecall;
                result.F1Score = aggregateMetrics.MacroF1;

                // Store additional metrics for thesis documentation
                result.MacroAccuracy = aggregateMetrics.MacroAccuracy;
                result.WeightedF1 = aggregateMetrics.WeightedF1;
                result.ConfusionMatrixJson = EvaluationMetricsExtractor.SerializeConfusionMatrix(metrics.ConfusionMatrix, classLabels);
                result.PerClassMetricsJson = System.Text.Json.JsonSerializer.Serialize(perClassMetrics, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                // Step 9: Save model and metadata
                if (!string.IsNullOrWhiteSpace(options.ModelOutputPath))
                {
                    await SaveModelAsync(trainedModel, dataView.Schema, options, metrics, dataList.Count).ConfigureAwait(false);
                    result.ModelPath = options.ModelOutputPath;

                    if (File.Exists(options.ModelOutputPath))
                    {
                        var fileInfo = new FileInfo(options.ModelOutputPath);
                        result.ModelSizeBytes = fileInfo.Length;
                    }
                }

                // Step 10: Extract class labels
                result.ClassLabels = ExtractClassLabels();

                stopwatch.Stop();
                result.TrainingDuration = stopwatch.Elapsed;
                result.Success = true;
                result.Message = $"Training completed successfully. Accuracy: {result.ValidationAccuracy:P2}";
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.Success = false;
                result.Message = $"Training failed: {ex.Message}";
                result.TrainingDuration = stopwatch.Elapsed;
            }

            return result;
        }

        private (IDataView TrainSet, IDataView TestSet) SplitData(IDataView data, double testFraction, int seed)
        {
            var split = _mlContext.Data.TrainTestSplit(data, testFraction: testFraction, seed: seed);
            return (split.TrainSet, split.TestSet);
        }

        private IEstimator<ITransformer> BuildTrainingPipeline(TrainingOptions options)
        {
            // Use CICIDS2017 preprocessing pipeline
            var preprocessingOptions = new PreprocessingOptions
            {
                EnableNormalization = options.EnableNormalization,
                ApplyLabelMapping = options.ApplyLabelMapping
            };

            var preprocessor = _preprocessingPipeline.BuildPipeline(preprocessingOptions);

            // Add FastTree trainer with One-vs-All strategy for multiclass classification
            var trainer = _mlContext.MulticlassClassification.Trainers.OneVersusAll(
                binaryEstimator: _mlContext.BinaryClassification.Trainers.FastTree(
                    numberOfLeaves: 20,
                    numberOfTrees: options.NumberOfTrees,
                    minimumExampleCountPerLeaf: 10
                ),
                labelColumnName: FeatureConfiguration.LabelColumnName
            );

            // Combine preprocessing and training
            var fullPipeline = preprocessor
                .Append(trainer)
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue(FeatureConfiguration.PredictedLabelColumnName));

            return fullPipeline;
        }

        private ITransformer TrainModel(IEstimator<ITransformer> pipeline, IDataView trainingData)
        {
            Console.WriteLine("Training model...");
            return pipeline.Fit(trainingData);
        }

        private MulticlassClassificationMetrics EvaluateModel(ITransformer model, IDataView testData)
        {
            Console.WriteLine("Evaluating model...");
            var predictions = model.Transform(testData);
            return _mlContext.MulticlassClassification.Evaluate(predictions, labelColumnName: FeatureConfiguration.LabelColumnName);
        }

        private async Task SaveModelAsync(ITransformer model, DataViewSchema schema, TrainingOptions options, MulticlassClassificationMetrics metrics, int totalSamples)
        {
            await _modelSaver.SaveModelAsync(model, schema, options.ModelOutputPath).ConfigureAwait(false);

            var metadataPath = Path.ChangeExtension(options.ModelOutputPath, ".metadata.json");

            // Extract correct metrics
            var classLabels = ExtractClassLabels();
            var perClassMetrics = EvaluationMetricsExtractor.ExtractPerClassMetrics(metrics.ConfusionMatrix, classLabels);
            var aggregateMetrics = EvaluationMetricsExtractor.CalculateAggregateMetrics(perClassMetrics, metrics.MicroAccuracy, metrics.MacroAccuracy);

            var metadata = new ModelMetadata
            {
                ModelName = "NetworkAttackClassifier",
                Version = "2.0.0",
                CreatedAt = DateTime.UtcNow,
                TrainedAt = DateTime.UtcNow,
                Algorithm = "FastTree + One-vs-All (Multiclass)",
                TrainingSamplesCount = (int)(totalSamples * (1.0 - options.TestSplit)),
                ValidationSamplesCount = (int)(totalSamples * options.TestSplit),
                // Correct metrics
                Accuracy = aggregateMetrics.MicroAccuracy,
                Precision = aggregateMetrics.MacroPrecision,
                Recall = aggregateMetrics.MacroRecall,
                F1Score = aggregateMetrics.MacroF1,
                // Additional metrics
                MacroAccuracy = aggregateMetrics.MacroAccuracy,
                WeightedF1 = aggregateMetrics.WeightedF1,
                ClassLabels = classLabels,
                DatasetName = options.DatasetName,
                DatasetVersion = options.DatasetVersion,
                FeatureCount = FeatureConfiguration.FeatureCount,
                OriginalLabelCount = 15,
                NormalizedLabelCount = _labelMapper.GetCategoryCount(),
                PreprocessingConfig = $"Normalization: {options.EnableNormalization}, LabelMapping: {options.ApplyLabelMapping}",
                NormalizationApplied = options.EnableNormalization,
                LabelMappingApplied = options.ApplyLabelMapping,
                // Confusion matrix and per-class metrics for thesis
                ConfusionMatrixJson = EvaluationMetricsExtractor.SerializeConfusionMatrix(metrics.ConfusionMatrix, classLabels),
                PerClassMetricsJson = System.Text.Json.JsonSerializer.Serialize(perClassMetrics, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }),
                HyperParameters = new Dictionary<string, string>
                {
                    ["NumberOfTrees"] = options.NumberOfTrees.ToString(),
                    ["RandomSeed"] = options.RandomSeed.ToString(),
                    ["TestSplit"] = options.TestSplit.ToString(),
                    ["NumberOfLeaves"] = "20",
                    ["MinimumExampleCountPerLeaf"] = "10"
                }
            };

            await _modelSaver.SaveMetadataAsync(metadata, metadataPath).ConfigureAwait(false);
            Console.WriteLine($"[ML] Model and metadata saved to: {options.ModelOutputPath}");
            Console.WriteLine($"[ML] Evaluation Results:");
            Console.WriteLine($"     Micro Accuracy: {aggregateMetrics.MicroAccuracy:P2}");
            Console.WriteLine($"     Macro Precision: {aggregateMetrics.MacroPrecision:F4}");
            Console.WriteLine($"     Macro Recall: {aggregateMetrics.MacroRecall:F4}");
            Console.WriteLine($"     Macro F1: {aggregateMetrics.MacroF1:F4}");
        }

        private double CalculateF1Score(double precision, double recall)
        {
            if (precision + recall == 0)
                return 0;

            return 2 * (precision * recall) / (precision + recall);
        }

        private string[] ExtractClassLabels()
        {
            // Return normalized CICIDS2017 labels (8 categories)
            return _labelMapper.GetNormalizedCategories().ToArray();
        }
    }
}
