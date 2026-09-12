using System;
using System.Collections.Generic;

namespace NetworkAttackDetectionPlatform.MachineLearning.Models
{
    /// <summary>
    /// Represents metadata about a trained ML model.
    /// Includes version, training date, performance metrics, and configuration.
    /// </summary>
    public sealed class ModelMetadata
    {
        public string ModelName { get; set; } = string.Empty;

        public string Version { get; set; } = "1.0.0";

        public DateTime CreatedAt { get; set; }

        public DateTime TrainedAt { get; set; }

        public string Algorithm { get; set; } = string.Empty;

        public int TrainingSamplesCount { get; set; }

        public int ValidationSamplesCount { get; set; }

        /// <summary>
        /// Micro Accuracy (correct metric).
        /// </summary>
        public double Accuracy { get; set; }

        /// <summary>
        /// Macro-averaged Precision (correct metric).
        /// </summary>
        public double Precision { get; set; }

        /// <summary>
        /// Macro-averaged Recall (correct metric).
        /// </summary>
        public double Recall { get; set; }

        /// <summary>
        /// Macro-averaged F1 Score (correct metric).
        /// </summary>
        public double F1Score { get; set; }

        /// <summary>
        /// Macro Accuracy for additional context.
        /// </summary>
        public double MacroAccuracy { get; set; }

        /// <summary>
        /// Weighted F1 Score (weighted by class support).
        /// </summary>
        public double WeightedF1 { get; set; }

        public string[] ClassLabels { get; set; } = Array.Empty<string>();

        public long ModelSizeBytes { get; set; }

        public string DatasetName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Dictionary<string, string> HyperParameters { get; set; } = new Dictionary<string, string>();

        // CICIDS2017-specific metadata
        public string DatasetVersion { get; set; } = string.Empty;

        public int FeatureCount { get; set; }

        public int OriginalLabelCount { get; set; }

        public int NormalizedLabelCount { get; set; }

        public string PreprocessingConfig { get; set; } = string.Empty;

        public bool NormalizationApplied { get; set; }

        public bool LabelMappingApplied { get; set; }

        /// <summary>
        /// Confusion matrix serialized as JSON for thesis documentation.
        /// </summary>
        public string ConfusionMatrixJson { get; set; } = string.Empty;

        /// <summary>
        /// Per-class metrics serialized as JSON for thesis documentation.
        /// </summary>
        public string PerClassMetricsJson { get; set; } = string.Empty;
    }
}
