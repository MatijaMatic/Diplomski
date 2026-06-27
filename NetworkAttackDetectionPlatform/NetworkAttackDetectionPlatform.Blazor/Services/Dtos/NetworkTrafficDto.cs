using System;

namespace NetworkAttackDetectionPlatform.Blazor.Services.Dtos
{
    public sealed class NetworkTrafficDto
    {
        public string SourceIp { get; set; } = string.Empty;
        public string DestinationIp { get; set; } = string.Empty;
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public int Protocol { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int PayloadSize { get; set; }
    }
}
