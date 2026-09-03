using System;
using NetworkAttackDetectionPlatform.Domain.Enums;

namespace NetworkAttackDetectionPlatform.MachineLearning.Utilities
{
    /// <summary>
    /// Maps CICIDS2017 normalized attack labels to application AttackType enum values.
    /// </summary>
    public static class LabelToAttackTypeMapper
    {
        /// <summary>
        /// Maps a normalized CICIDS2017 label to AttackTypeEnum.
        /// </summary>
        /// <param name="label">The normalized attack label (e.g., "DDoS", "PortScan", "BruteForce", "BENIGN").</param>
        /// <returns>The corresponding AttackTypeEnum value.</returns>
        public static AttackTypeEnum MapLabelToAttackType(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                return AttackTypeEnum.Unknown;

            return label.Trim() switch
            {
                "BENIGN" => AttackTypeEnum.Unknown,
                "PortScan" => AttackTypeEnum.PortScan,
                "DDoS" => AttackTypeEnum.DDoS,
                "BruteForce" => AttackTypeEnum.BruteForce,
                "WebAttack" => AttackTypeEnum.Malware, // Web attacks mapped to Malware
                "Bot" => AttackTypeEnum.Malware,
                "Infiltration" => AttackTypeEnum.DataExfiltration,
                "Other" => AttackTypeEnum.Unknown,
                _ => AttackTypeEnum.Unknown
            };
        }

        /// <summary>
        /// Maps confidence score to a severity level.
        /// Higher confidence indicates higher severity.
        /// </summary>
        /// <param name="confidence">Confidence value (0-100).</param>
        /// <param name="attackType">The attack type (used to determine base severity).</param>
        /// <returns>A severity level (0-4).</returns>
        public static int MapConfidenceToSeverity(double confidence, AttackTypeEnum attackType)
        {
            // Clamp confidence to 0-100 range
            var clampedConfidence = Math.Clamp(confidence, 0.0, 100.0);

            // BENIGN traffic is always low severity
            if (attackType == AttackTypeEnum.Unknown)
                return (int)SeverityLevelEnum.Low;

            // For attack types, map confidence to severity
            return clampedConfidence switch
            {
                >= 90 => (int)SeverityLevelEnum.Critical,      // Very high confidence
                >= 75 => (int)SeverityLevelEnum.High,          // High confidence
                >= 50 => (int)SeverityLevelEnum.Medium,        // Moderate confidence
                >= 25 => (int)SeverityLevelEnum.Low,           // Low confidence
                _ => (int)SeverityLevelEnum.Unknown             // Very low or unknown
            };
        }
    }
}
