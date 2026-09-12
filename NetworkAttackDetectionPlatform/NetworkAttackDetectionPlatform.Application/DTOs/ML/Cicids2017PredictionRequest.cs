using System;
using System.ComponentModel.DataAnnotations;

namespace NetworkAttackDetectionPlatform.Application.DTOs.ML
{
    /// <summary>
    /// Complete CICIDS2017 network traffic record with all 78 flow features.
    /// Designed for scientifically valid ML-based attack classification.
    /// All 78 features MUST be provided; missing values are NOT accepted.
    /// 
    /// Feature order and indices must match FeatureConfiguration.NumericalFeatures exactly.
    /// </summary>
    public sealed class Cicids2017PredictionRequest
    {
        // Identifiers (not ML features)
        [Required]
        [StringLength(45)]
        public string SourceIp { get; set; } = string.Empty;

        [Required]
        [StringLength(45)]
        public string DestinationIp { get; set; } = string.Empty;

        // Feature 0: DestinationPort
        [Required]
        [Range(0, 65535)]
        public float DestinationPort { get; set; }

        // Feature 1: FlowDuration
        [Required]
        public float FlowDuration { get; set; }

        // Feature 2: TotalFwdPackets
        [Required]
        public float TotalFwdPackets { get; set; }

        // Feature 3: TotalBackwardPackets
        [Required]
        public float TotalBackwardPackets { get; set; }

        // Feature 4: TotalLengthOfFwdPackets
        [Required]
        public float TotalLengthOfFwdPackets { get; set; }

        // Feature 5: TotalLengthOfBwdPackets
        [Required]
        public float TotalLengthOfBwdPackets { get; set; }

        // Feature 6: FwdPacketLengthMax
        [Required]
        public float FwdPacketLengthMax { get; set; }

        // Feature 7: FwdPacketLengthMin
        [Required]
        public float FwdPacketLengthMin { get; set; }

        // Feature 8: FwdPacketLengthMean
        [Required]
        public float FwdPacketLengthMean { get; set; }

        // Feature 9: FwdPacketLengthStd
        [Required]
        public float FwdPacketLengthStd { get; set; }

        // Feature 10: BwdPacketLengthMax
        [Required]
        public float BwdPacketLengthMax { get; set; }

        // Feature 11: BwdPacketLengthMin
        [Required]
        public float BwdPacketLengthMin { get; set; }

        // Feature 12: BwdPacketLengthMean
        [Required]
        public float BwdPacketLengthMean { get; set; }

        // Feature 13: BwdPacketLengthStd
        [Required]
        public float BwdPacketLengthStd { get; set; }

        // Feature 14: FlowBytesPerSecond
        [Required]
        public float FlowBytesPerSecond { get; set; }

        // Feature 15: FlowPacketsPerSecond
        [Required]
        public float FlowPacketsPerSecond { get; set; }

        // Feature 16: FlowIATMean
        [Required]
        public float FlowIATMean { get; set; }

        // Feature 17: FlowIATStd
        [Required]
        public float FlowIATStd { get; set; }

        // Feature 18: FlowIATMax
        [Required]
        public float FlowIATMax { get; set; }

        // Feature 19: FlowIATMin
        [Required]
        public float FlowIATMin { get; set; }

        // Feature 20: FwdIATTotal
        [Required]
        public float FwdIATTotal { get; set; }

        // Feature 21: FwdIATMean
        [Required]
        public float FwdIATMean { get; set; }

        // Feature 22: FwdIATStd
        [Required]
        public float FwdIATStd { get; set; }

        // Feature 23: FwdIATMax
        [Required]
        public float FwdIATMax { get; set; }

        // Feature 24: FwdIATMin
        [Required]
        public float FwdIATMin { get; set; }

        // Feature 25: BwdIATTotal
        [Required]
        public float BwdIATTotal { get; set; }

        // Feature 26: BwdIATMean
        [Required]
        public float BwdIATMean { get; set; }

        // Feature 27: BwdIATStd
        [Required]
        public float BwdIATStd { get; set; }

        // Feature 28: BwdIATMax
        [Required]
        public float BwdIATMax { get; set; }

        // Feature 29: BwdIATMin
        [Required]
        public float BwdIATMin { get; set; }

        // Feature 30: FwdPSHFlags
        [Required]
        public float FwdPSHFlags { get; set; }

        // Feature 31: BwdPSHFlags
        [Required]
        public float BwdPSHFlags { get; set; }

        // Feature 32: FwdURGFlags
        [Required]
        public float FwdURGFlags { get; set; }

        // Feature 33: BwdURGFlags
        [Required]
        public float BwdURGFlags { get; set; }

        // Feature 34: FINFlagCount
        [Required]
        public float FINFlagCount { get; set; }

        // Feature 35: SYNFlagCount
        [Required]
        public float SYNFlagCount { get; set; }

        // Feature 36: RSTFlagCount
        [Required]
        public float RSTFlagCount { get; set; }

        // Feature 37: PSHFlagCount
        [Required]
        public float PSHFlagCount { get; set; }

        // Feature 38: ACKFlagCount
        [Required]
        public float ACKFlagCount { get; set; }

        // Feature 39: URGFlagCount
        [Required]
        public float URGFlagCount { get; set; }

        // Feature 40: CWEFlagCount
        [Required]
        public float CWEFlagCount { get; set; }

        // Feature 41: ECEFlagCount
        [Required]
        public float ECEFlagCount { get; set; }

        // Feature 42: FwdHeaderLength
        [Required]
        public float FwdHeaderLength { get; set; }

        // Feature 43: BwdHeaderLength
        [Required]
        public float BwdHeaderLength { get; set; }

        // Feature 44: FwdPacketsPerSecond
        [Required]
        public float FwdPacketsPerSecond { get; set; }

        // Feature 45: BwdPacketsPerSecond
        [Required]
        public float BwdPacketsPerSecond { get; set; }

        // Feature 46: MinPacketLength
        [Required]
        public float MinPacketLength { get; set; }

        // Feature 47: MaxPacketLength
        [Required]
        public float MaxPacketLength { get; set; }

        // Feature 48: PacketLengthMean
        [Required]
        public float PacketLengthMean { get; set; }

        // Feature 49: PacketLengthStd
        [Required]
        public float PacketLengthStd { get; set; }

        // Feature 50: PacketLengthVariance
        [Required]
        public float PacketLengthVariance { get; set; }

        // Feature 51: DownUpRatio
        [Required]
        public float DownUpRatio { get; set; }

        // Feature 52: AveragePacketSize
        [Required]
        public float AveragePacketSize { get; set; }

        // Feature 53: AvgFwdSegmentSize
        [Required]
        public float AvgFwdSegmentSize { get; set; }

        // Feature 54: AvgBwdSegmentSize
        [Required]
        public float AvgBwdSegmentSize { get; set; }

        // Feature 55: FwdAvgBytesPerBulk
        [Required]
        public float FwdAvgBytesPerBulk { get; set; }

        // Feature 56: FwdAvgPacketsPerBulk
        [Required]
        public float FwdAvgPacketsPerBulk { get; set; }

        // Feature 57: FwdAvgBulkRate
        [Required]
        public float FwdAvgBulkRate { get; set; }

        // Feature 58: BwdAvgBytesPerBulk
        [Required]
        public float BwdAvgBytesPerBulk { get; set; }

        // Feature 59: BwdAvgPacketsPerBulk
        [Required]
        public float BwdAvgPacketsPerBulk { get; set; }

        // Feature 60: BwdAvgBulkRate
        [Required]
        public float BwdAvgBulkRate { get; set; }

        // Feature 61: SubflowFwdPackets
        [Required]
        public float SubflowFwdPackets { get; set; }

        // Feature 62: SubflowFwdBytes
        [Required]
        public float SubflowFwdBytes { get; set; }

        // Feature 63: SubflowBwdPackets
        [Required]
        public float SubflowBwdPackets { get; set; }

        // Feature 64: SubflowBwdBytes
        [Required]
        public float SubflowBwdBytes { get; set; }

        // Feature 65: InitWinBytesForward
        [Required]
        public float InitWinBytesForward { get; set; }

        // Feature 66: InitWinBytesBackward
        [Required]
        public float InitWinBytesBackward { get; set; }

        // Feature 67: ActiveMean
        [Required]
        public float ActiveMean { get; set; }

        // Feature 68: ActiveStd
        [Required]
        public float ActiveStd { get; set; }

        // Feature 69: ActiveMax
        [Required]
        public float ActiveMax { get; set; }

        // Feature 70: ActiveMin
        [Required]
        public float ActiveMin { get; set; }

        // Feature 71: IdleMean
        [Required]
        public float IdleMean { get; set; }

        // Feature 72: IdleStd
        [Required]
        public float IdleStd { get; set; }

        // Feature 73: IdleMax
        [Required]
        public float IdleMax { get; set; }

        // Feature 74: IdleMin
        [Required]
        public float IdleMin { get; set; }

        // Feature 75: ActDataPktFwd
        [Required]
        public float ActDataPktFwd { get; set; }

        // Feature 76: MinSegSizeForward
        [Required]
        public float MinSegSizeForward { get; set; }

        // Feature 77: Protocol
        [Required]
        [Range(0, 255)]
        public float Protocol { get; set; }
    }
}
