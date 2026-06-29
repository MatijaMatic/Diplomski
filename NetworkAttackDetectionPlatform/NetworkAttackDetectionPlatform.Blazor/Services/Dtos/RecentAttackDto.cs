using System;

namespace NetworkAttackDetectionPlatform.Blazor.Services.Dtos
{
    public sealed class RecentAttackDto
    {
        public Guid Id { get; set; }
        public string SourceIp { get; set; } = string.Empty;
        public string DestinationIp { get; set; } = string.Empty;
        public int AttackType { get; set; }
        public int Severity { get; set; }
        public double Confidence { get; set; }
        public int Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
