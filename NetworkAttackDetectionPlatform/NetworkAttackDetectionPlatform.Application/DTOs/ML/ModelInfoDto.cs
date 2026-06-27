using System;

namespace NetworkAttackDetectionPlatform.Application.DTOs.ML
{
    public sealed class ModelInfoDto
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = "0.0";
        public DateTime TrainedAt { get; set; }
        public long SizeBytes { get; set; }
    }
}
