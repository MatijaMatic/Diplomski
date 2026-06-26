using System;

namespace NetworkAttackDetectionPlatform.Application.DTOs
{
    public sealed class CreateRecommendationDto
    {
        public Guid AttackDetectionId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}