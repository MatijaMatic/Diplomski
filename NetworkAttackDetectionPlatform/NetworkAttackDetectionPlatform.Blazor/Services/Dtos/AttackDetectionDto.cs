using System;
using System.Collections.Generic;

namespace NetworkAttackDetectionPlatform.Blazor.Services.Dtos
{
    public sealed class AttackDetectionDto
    {
        public Guid Id { get; set; }
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
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<RecommendationDto> Recommendations { get; set; } = new List<RecommendationDto>();
    }
}