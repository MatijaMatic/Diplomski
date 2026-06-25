using System;
using System.Net;
using NetworkAttackDetectionPlatform.Domain.Constants;

namespace NetworkAttackDetectionPlatform.Domain.ValueObjects
{
    public sealed record IpAddress
    {
        public string Value { get; }

        public IpAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("IP address cannot be empty.", nameof(value));

            if (value.Length > DomainConstants.MaxIpLength)
                throw new ArgumentException("IP address is too long.", nameof(value));

            if (!System.Net.IPAddress.TryParse(value, out _))
                throw new ArgumentException("Invalid IP address format.", nameof(value));

            Value = value;
        }

        public override string ToString() => Value;
    }
}