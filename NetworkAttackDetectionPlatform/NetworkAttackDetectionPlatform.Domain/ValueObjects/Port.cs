using System;
using NetworkAttackDetectionPlatform.Domain.Constants;

namespace NetworkAttackDetectionPlatform.Domain.ValueObjects
{
    public sealed record Port
    {
        public int Value { get; }

        public Port(int value)
        {
            if (value < DomainConstants.MinPort || value > DomainConstants.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(value), $"Port must be between {DomainConstants.MinPort} and {DomainConstants.MaxPort}.");

            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}