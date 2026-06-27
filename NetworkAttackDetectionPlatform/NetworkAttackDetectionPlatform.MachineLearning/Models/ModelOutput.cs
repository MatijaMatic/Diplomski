using Microsoft.ML.Data;

namespace NetworkAttackDetectionPlatform.MachineLearning.Models
{
    /// <summary>
    /// Represents the output schema for the ML model.
    /// Contains the predicted label and confidence scores.
    /// </summary>
    public sealed class ModelOutput
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; } = string.Empty;

        [ColumnName("Score")]
        public float[] Score { get; set; } = Array.Empty<float>();

        [ColumnName("Probability")]
        public float Probability { get; set; }
    }
}
