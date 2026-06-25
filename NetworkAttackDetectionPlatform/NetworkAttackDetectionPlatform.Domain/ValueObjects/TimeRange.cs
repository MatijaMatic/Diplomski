using System;

namespace NetworkAttackDetectionPlatform.Domain.ValueObjects
{
    public sealed record TimeRange
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        public TimeRange(DateTime start, DateTime end)
        {
            // Use UTC to avoid ambiguity in domain
            var s = start.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(start, DateTimeKind.Utc) : start.ToUniversalTime();
            var e = end.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(end, DateTimeKind.Utc) : end.ToUniversalTime();

            if (s > e)
                throw new ArgumentException("Start time must be earlier than or equal to end time.");

            Start = s;
            End = e;
        }

        public override string ToString() => $"{Start:o}/{End:o}";
    }
}