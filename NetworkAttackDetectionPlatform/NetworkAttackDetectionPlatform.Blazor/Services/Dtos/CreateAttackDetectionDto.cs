using System;

namespace NetworkAttackDetectionPlatform.Blazor.Services.Dtos
{
    public sealed class CreateAttackDetectionDto
    {
        public string SourceIp { get; set; } = string.Empty;
        public string DestinationIp { get; set; } = string.Empty;
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public int Protocol { get; set; }
        public int AttackType { get; set; }
        public int Severity { get; set; }
        public double Confidence { get; set; }
        public DateTime OccurrenceStart { get; set; }
        public DateTime OccurrenceEnd { get; set; }
    }
}