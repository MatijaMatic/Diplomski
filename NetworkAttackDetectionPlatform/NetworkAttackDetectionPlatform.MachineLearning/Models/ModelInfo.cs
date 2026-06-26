using System;

namespace NetworkAttackDetectionPlatform.MachineLearning.Models
{
    public sealed class ModelInfo
    {
        public string Name { get; init; } = string.Empty;
        public string Version { get; init; } = "0.0";
        public DateTime TrainedAt { get; init; }
        public long SizeBytes { get; init; }
    }
}
