using System;

namespace NetworkAttackDetectionPlatform.Application.DTOs.Dashboard
{
    public sealed class AttackTimelineDto
    {
        public string Date { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
