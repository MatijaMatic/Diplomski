namespace NetworkAttackDetectionPlatform.Blazor.Services.Dtos
{
    public sealed class AttackTimelineDto
    {
        public string Date { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
