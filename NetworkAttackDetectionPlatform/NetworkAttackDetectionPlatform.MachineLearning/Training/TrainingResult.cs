using System;

namespace NetworkAttackDetectionPlatform.MachineLearning.Training
{
    /// <summary>
    /// Represents the result of a model training operation.
    /// Contains training metrics, validation metrics, and metadata.
    /// </summary>
    public sealed class TrainingResult
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime TrainedAt { get; set; }

        public TimeSpan TrainingDuration { get; set; }

        public int TrainingSamplesCount { get; set; }

        public int ValidationSamplesCount { get; set; }

        public double TrainingAccuracy { get; set; }

        public double ValidationAccuracy { get; set; }

        /// <summary>
        /// Macro-averaged precision (correct average across all classes).
        /// </summary>
        public double Precision { get; set; }

        /// <summary>
        /// Macro-averaged recall (correct average across all classes).
        /// </summary>
        public double Recall { get; set; }

        /// <summary>
        /// Macro-averaged F1 score (correct average across all classes).
        /// </summary>
        public double F1Score { get; set; }

        /// <summary>
        /// Macro-averaged accuracy.
        /// </summary>
        public double MacroAccuracy { get; set; }

        /// <summary>
        /// Weighted F1 score (weighted by class support).
        /// </summary>
        public double WeightedF1 { get; set; }

        public string ModelPath { get; set; } = string.Empty;

        public long ModelSizeBytes { get; set; }

        public string[] ClassLabels { get; set; } = Array.Empty<string>();

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
