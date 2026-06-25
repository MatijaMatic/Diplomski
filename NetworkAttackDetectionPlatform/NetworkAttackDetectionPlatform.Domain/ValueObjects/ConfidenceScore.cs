using System;
using NetworkAttackDetectionPlatform.Domain.Constants;

namespace NetworkAttackDetectionPlatform.Domain.ValueObjects
{
    public sealed record ConfidenceScore
    {
        public double Value { get; }

        public ConfidenceScore(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Confidence score must be a finite number.", nameof(value));

            if (value < DomainConstants.MinConfidence || value > DomainConstants.MaxConfidence)
                throw new ArgumentOutOfRangeException(nameof(value), $"Confidence score must be between {DomainConstants.MinConfidence} and {DomainConstants.MaxConfidence}.");

            Value = value;
        }

        public override string ToString() => Value.ToString("F2");
    }
}