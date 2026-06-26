using System;

namespace NetworkAttackDetectionPlatform.Blazor.Services.Dtos
{
    public sealed class CreateRecommendationDto
    {
        public Guid AttackDetectionId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
