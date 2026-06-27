namespace NetworkAttackDetectionPlatform.MachineLearning.Prediction;

public sealed class PredictionResult
{
    public string Label { get; set; } = string.Empty;

    public double Score { get; set; }

    public bool IsAnomaly { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}