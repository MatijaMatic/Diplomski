using System;

namespace NetworkAttackDetectionPlatform.Application.DTOs.ML
{
    /// <summary>
    /// Result of attack classification.
    /// Indicates whether the prediction is from a real trained ML model or a fallback/demo provider.
    /// </summary>
    public sealed class AttackClassificationResultDto
    {
        public Guid Id { get; set; }
        public int AttackType { get; set; }
        public int Severity { get; set; }
        public double Confidence { get; set; }
        public DateTime DetectedAt { get; set; }

        /// <summary>
        /// Indicates whether this prediction is from a trained ML model (true) or fallback/demo logic (false).
        /// Use this to determine whether to trust the prediction for scientific purposes.
        /// </summary>
        public bool IsMLBacked { get; set; } = false;
    }
}
