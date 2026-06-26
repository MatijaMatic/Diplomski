using System;

namespace NetworkAttackDetectionPlatform.Application.DTOs
{
    public sealed class RecommendationDto
    {
        public Guid Id { get; set; }
        public Guid AttackDetectionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}