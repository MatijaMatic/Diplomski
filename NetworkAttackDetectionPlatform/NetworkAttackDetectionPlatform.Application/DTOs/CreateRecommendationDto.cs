using System;
using System.ComponentModel.DataAnnotations;

namespace NetworkAttackDetectionPlatform.Application.DTOs
{
    public sealed class CreateRecommendationDto
    {
        [Required]
        public Guid AttackDetectionId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Text { get; set; } = string.Empty;
    }
}