using System;
using System.ComponentModel.DataAnnotations;

namespace NetworkAttackDetectionPlatform.Application.DTOs
{
    public sealed class CreateAttackDetectionDto
    {
        [Required]
        [StringLength(45)]
        public string SourceIp { get; set; } = string.Empty;

        [Required]
        [StringLength(45)]
        public string DestinationIp { get; set; } = string.Empty;

        [Range(1, 65535)]
        public int SourcePort { get; set; }

        [Range(1, 65535)]
        public int DestinationPort { get; set; }

        public int Protocol { get; set; }

        public int AttackType { get; set; }

        public int Severity { get; set; }

        [Range(0.0, 100.0)]
        public double Confidence { get; set; }

        public DateTime OccurrenceStart { get; set; }

        public DateTime OccurrenceEnd { get; set; }
    }
}