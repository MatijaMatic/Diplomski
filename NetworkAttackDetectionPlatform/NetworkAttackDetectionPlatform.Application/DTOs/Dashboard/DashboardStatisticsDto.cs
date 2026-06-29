namespace NetworkAttackDetectionPlatform.Application.DTOs.Dashboard
{
    public sealed class DashboardStatisticsDto
    {
        public int TotalAttacks { get; set; }
        public int AttacksToday { get; set; }
        public int CriticalAttacks { get; set; }
        public double AverageConfidence { get; set; }
        public int MostCommonAttackType { get; set; }
    }
}
