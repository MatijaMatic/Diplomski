using System;

namespace NetworkAttackDetectionPlatform.Application.DTOs.ML
{
    public sealed class PredictionResultDto
    {
        public Guid Id { get; set; }
        public double Confidence { get; set; }
        public string Label { get; set; } = string.Empty; // e.g., "Attack" or "Benign"
        public DateTime PredictedAt { get; set; }
    }
}