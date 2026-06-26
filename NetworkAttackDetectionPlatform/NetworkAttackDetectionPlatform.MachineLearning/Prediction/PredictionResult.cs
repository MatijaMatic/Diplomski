namespace NetworkAttackDetectionPlatform.MachineLearning.Prediction
{
    public sealed class PredictionResult
    {
        public string Label { get; init; } = string.Empty;
        public double Score { get; init; }
        public bool IsAnomaly { get; init; }
    }
}
