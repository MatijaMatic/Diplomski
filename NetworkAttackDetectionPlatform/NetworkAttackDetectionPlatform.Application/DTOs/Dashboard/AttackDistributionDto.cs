namespace NetworkAttackDetectionPlatform.Application.DTOs.Dashboard
{
    public sealed class AttackDistributionDto
    {
        public string AttackType { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
