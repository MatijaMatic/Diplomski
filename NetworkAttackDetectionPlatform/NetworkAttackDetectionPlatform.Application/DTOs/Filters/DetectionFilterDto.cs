using System;

namespace NetworkAttackDetectionPlatform.Application.DTOs.Filters
{
    public sealed class DetectionFilterDto
    {
        public int? AttackType { get; set; }
        public int? Severity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? MinConfidence { get; set; }
        public double? MaxConfidence { get; set; }
    }
}
