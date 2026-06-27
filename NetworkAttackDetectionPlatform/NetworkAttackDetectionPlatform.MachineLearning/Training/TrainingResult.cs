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

        public double Precision { get; set; }

        public double Recall { get; set; }

        public double F1Score { get; set; }

        public string ModelPath { get; set; } = string.Empty;

        public long ModelSizeBytes { get; set; }

        public string[] ClassLabels { get; set; } = Array.Empty<string>();
    }
}
