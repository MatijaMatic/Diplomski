namespace NetworkAttackDetectionPlatform.Application.DTOs.Dashboard
{
    public sealed class SeverityDistributionDto
    {
        public int Critical { get; set; }
        public int High { get; set; }
        public int Medium { get; set; }
        public int Low { get; set; }
        public int Unknown { get; set; }
    }
}
