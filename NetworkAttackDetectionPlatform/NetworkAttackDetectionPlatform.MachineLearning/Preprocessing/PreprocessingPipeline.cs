using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace NetworkAttackDetectionPlatform.MachineLearning.Preprocessing;

/// <summary>
/// Builds ML.NET preprocessing pipelines for CICIDS2017 data preparation.
/// Handles missing values, normalization, and feature transformation for FastTree training.
/// </summary>
public class PreprocessingPipeline
{
    private readonly MLContext _mlContext;
    private readonly LabelMapper _labelMapper;

    public PreprocessingPipeline(MLContext mlContext)
    {
        _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
        _labelMapper = new LabelMapper();
    }

    /// <summary>
    /// Builds a complete preprocessing pipeline for CICIDS2017 training data.
    /// </summary>
    /// <param name="options">Preprocessing options.</param>
    /// <returns>An ML.NET data transformation estimator.</returns>
    public IEstimator<ITransformer> BuildPipeline(PreprocessingOptions? options = null)
    {
        options ??= new PreprocessingOptions();

        // Step 1: Handle missing and invalid values
        IEstimator<ITransformer> pipeline = _mlContext.Transforms.ReplaceMissingValues(
            outputColumnName: FeatureConfiguration.NumericalFeatures[0],
            inputColumnName: FeatureConfiguration.NumericalFeatures[0],
            replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean);

        // Replace missing values for all numerical features
        foreach (var feature in FeatureConfiguration.NumericalFeatures.Skip(1))
        {
            pipeline = pipeline.Append(
                _mlContext.Transforms.ReplaceMissingValues(
                    outputColumnName: feature,
                    inputColumnName: feature,
                    replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean));
        }

        // Step 2: Concatenate all numerical features into a single feature vector
        pipeline = pipeline.Append(
            _mlContext.Transforms.Concatenate(
                outputColumnName: FeatureConfiguration.FeaturesColumnName,
                inputColumnNames: FeatureConfiguration.NumericalFeatures));

        // Step 3: Apply normalization if enabled (after concatenation)
        if (options.EnableNormalization)
        {
            pipeline = pipeline.Append(
                _mlContext.Transforms.NormalizeMinMax(
                    outputColumnName: FeatureConfiguration.FeaturesColumnName,
                    inputColumnName: FeatureConfiguration.FeaturesColumnName));
        }

        // Step 4: Convert labels to keys (categorical encoding for multi-class classification)
        pipeline = pipeline.Append(
            _mlContext.Transforms.Conversion.MapValueToKey(
                outputColumnName: FeatureConfiguration.LabelColumnName,
                inputColumnName: FeatureConfiguration.LabelColumnName));

        return pipeline;
    }

    /// <summary>
    /// Builds a simplified preprocessing pipeline for prediction (inference).
    /// Does not include label transformation since labels are not available during prediction.
    /// </summary>
    /// <param name="options">Preprocessing options.</param>
    /// <returns>An ML.NET data transformation estimator.</returns>
    public IEstimator<ITransformer> BuildPredictionPipeline(PreprocessingOptions? options = null)
    {
        options ??= new PreprocessingOptions();

        // Step 1: Handle missing and invalid values
        IEstimator<ITransformer> pipeline = _mlContext.Transforms.ReplaceMissingValues(
            outputColumnName: FeatureConfiguration.NumericalFeatures[0],
            inputColumnName: FeatureConfiguration.NumericalFeatures[0],
            replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean);

        foreach (var feature in FeatureConfiguration.NumericalFeatures.Skip(1))
        {
            pipeline = pipeline.Append(
                _mlContext.Transforms.ReplaceMissingValues(
                    outputColumnName: feature,
                    inputColumnName: feature,
                    replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean));
        }

        // Step 2: Concatenate features
        pipeline = pipeline.Append(
            _mlContext.Transforms.Concatenate(
                outputColumnName: FeatureConfiguration.FeaturesColumnName,
                inputColumnNames: FeatureConfiguration.NumericalFeatures));

        // Step 3: Apply normalization if enabled (after concatenation)
        if (options.EnableNormalization)
        {
            pipeline = pipeline.Append(
                _mlContext.Transforms.NormalizeMinMax(
                    outputColumnName: FeatureConfiguration.FeaturesColumnName,
                    inputColumnName: FeatureConfiguration.FeaturesColumnName));
        }

        return pipeline;
    }

    /// <summary>
    /// Creates a custom preprocessing transformer that applies label mapping.
    /// This is used to normalize CICIDS2017 labels before ML.NET processing.
    /// </summary>
    /// <param name="data">Input data with original labels.</param>
    /// <returns>Transformed data with normalized labels.</returns>
    public IDataView ApplyLabelMapping(IDataView data)
    {
        // Note: This is a placeholder for label mapping logic.
        // In production, you would use a custom ML.NET transformer or
        // pre-process data before loading into IDataView.
        // For now, this method demonstrates the intended architecture.
        return data;
    }

    /// <summary>
    /// Validates that all required features are present in the data schema.
    /// </summary>
    /// <param name="data">The input data view.</param>
    /// <returns>True if all required features are present.</returns>
    public bool ValidateSchema(IDataView data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var schema = data.Schema;
        var missingFeatures = new List<string>();

        foreach (var feature in FeatureConfiguration.NumericalFeatures)
        {
            if (!schema.Any(col => col.Name.Equals(feature, StringComparison.OrdinalIgnoreCase)))
            {
                missingFeatures.Add(feature);
            }
        }

        if (!schema.Any(col => col.Name.Equals(FeatureConfiguration.LabelColumnName, StringComparison.OrdinalIgnoreCase)))
        {
            missingFeatures.Add(FeatureConfiguration.LabelColumnName);
        }

        if (missingFeatures.Any())
        {
            var message = $"Missing required features: {string.Join(", ", missingFeatures)}";
            throw new InvalidOperationException(message);
        }

        return true;
    }

    /// <summary>
    /// Gets statistics about the preprocessing pipeline.
    /// </summary>
    /// <returns>Preprocessing statistics.</returns>
    public PreprocessingStatistics GetStatistics()
    {
        return new PreprocessingStatistics
        {
            TotalFeatures = FeatureConfiguration.FeatureCount,
            NumericalFeatures = FeatureConfiguration.NumericalFeatures.Length,
            CountFeatures = FeatureConfiguration.CountFeatures.Length,
            ContinuousFeatures = FeatureConfiguration.ContinuousFeatures.Length,
            SupportedCategories = _labelMapper.GetCategoryCount()
        };
    }
}

/// <summary>
/// Configuration options for the preprocessing pipeline.
/// </summary>
public class PreprocessingOptions
{
    /// <summary>
    /// Enable min-max normalization of features.
    /// </summary>
    public bool EnableNormalization { get; set; } = true;

    /// <summary>
    /// Missing value replacement strategy.
    /// </summary>
    public MissingValueReplacingEstimator.ReplacementMode MissingValueStrategy { get; set; } 
        = MissingValueReplacingEstimator.ReplacementMode.Mean;

    /// <summary>
    /// Whether to apply label mapping before training.
    /// </summary>
    public bool ApplyLabelMapping { get; set; } = true;

    /// <summary>
    /// Whether to remove outliers during preprocessing.
    /// </summary>
    public bool RemoveOutliers { get; set; } = false;

    /// <summary>
    /// Outlier threshold (z-score) if outlier removal is enabled.
    /// </summary>
    public double OutlierThreshold { get; set; } = 3.0;
}

/// <summary>
/// Statistics about the preprocessing pipeline configuration.
/// </summary>
public class PreprocessingStatistics
{
    public int TotalFeatures { get; set; }
    public int NumericalFeatures { get; set; }
    public int CountFeatures { get; set; }
    public int ContinuousFeatures { get; set; }
    public int SupportedCategories { get; set; }

    public override string ToString()
    {
        return $"Preprocessing Stats: {TotalFeatures} total features " +
               $"({NumericalFeatures} numerical, {CountFeatures} count, {ContinuousFeatures} continuous), " +
               $"{SupportedCategories} attack categories";
    }
}
