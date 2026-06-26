using System;

namespace NetworkAttackDetectionPlatform.Application.DTOs.ML
{
    public sealed class AttackClassificationResultDto
    {
        public Guid Id { get; set; }
        public int AttackType { get; set; }
        public int Severity { get; set; }
        public double Confidence { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
