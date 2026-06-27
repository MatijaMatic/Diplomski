using Microsoft.ML.Data;

namespace NetworkAttackDetectionPlatform.MachineLearning.Models
{
    /// <summary>
    /// Represents the input schema for the ML model.
    /// This is the processed/engineered feature vector that the model consumes.
    /// </summary>
    public sealed class ModelInput
    {
        [ColumnName("Features")]
        [VectorType(9)]
        public float[] Features { get; set; } = new float[9];

        [ColumnName("Label")]
        public string Label { get; set; } = string.Empty;
    }
}
