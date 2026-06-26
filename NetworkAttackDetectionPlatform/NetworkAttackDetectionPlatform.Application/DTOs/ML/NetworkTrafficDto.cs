using System;

namespace NetworkAttackDetectionPlatform.Application.DTOs.ML
{
    public sealed class NetworkTrafficDto
    {
        public string SourceIp { get; set; } = string.Empty;
        public string DestinationIp { get; set; } = string.Empty;
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public int Protocol { get; set; }
        public DateTime Timestamp { get; set; }
        public int PayloadSize { get; set; }
        // Add other features as needed
    }
}