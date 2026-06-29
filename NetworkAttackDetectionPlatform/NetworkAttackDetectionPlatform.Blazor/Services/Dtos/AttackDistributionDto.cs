namespace NetworkAttackDetectionPlatform.Blazor.Services.Dtos
{
    public sealed class AttackDistributionDto
    {
        public string AttackType { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
