using Microsoft.ML.Data;

namespace NetworkAttackDetectionPlatform.MachineLearning.Models
{
    /// <summary>
    /// Represents the complete CICIDS2017 dataset feature set for training.
    /// 78 numerical features (indices 0..77) followed by Label at index 78.
    /// </summary>
    public sealed class Cicids2017TrainingData
    {
        // Basic Connection Features
        [LoadColumn(0)]
        [ColumnName("DestinationPort")]
        public float DestinationPort { get; set; }

        [LoadColumn(1)]
        [ColumnName("FlowDuration")]
        public float FlowDuration { get; set; }

        [LoadColumn(2)]
        [ColumnName("TotalFwdPackets")]
        public float TotalFwdPackets { get; set; }

        [LoadColumn(3)]
        [ColumnName("TotalBackwardPackets")]
        public float TotalBackwardPackets { get; set; }

        [LoadColumn(4)]
        [ColumnName("TotalLengthOfFwdPackets")]
        public float TotalLengthOfFwdPackets { get; set; }

        [LoadColumn(5)]
        [ColumnName("TotalLengthOfBwdPackets")]
        public float TotalLengthOfBwdPackets { get; set; }

        // Packet Length Statistics
        [LoadColumn(6)]
        [ColumnName("FwdPacketLengthMax")]
        public float FwdPacketLengthMax { get; set; }

        [LoadColumn(7)]
        [ColumnName("FwdPacketLengthMin")]
        public float FwdPacketLengthMin { get; set; }

        [LoadColumn(8)]
        [ColumnName("FwdPacketLengthMean")]
        public float FwdPacketLengthMean { get; set; }

        [LoadColumn(9)]
        [ColumnName("FwdPacketLengthStd")]
        public float FwdPacketLengthStd { get; set; }

        [LoadColumn(10)]
        [ColumnName("BwdPacketLengthMax")]
        public float BwdPacketLengthMax { get; set; }

        [LoadColumn(11)]
        [ColumnName("BwdPacketLengthMin")]
        public float BwdPacketLengthMin { get; set; }

        [LoadColumn(12)]
        [ColumnName("BwdPacketLengthMean")]
        public float BwdPacketLengthMean { get; set; }

        [LoadColumn(13)]
        [ColumnName("BwdPacketLengthStd")]
        public float BwdPacketLengthStd { get; set; }

        // Flow Rate Features
        [LoadColumn(14)]
        [ColumnName("FlowBytesPerSecond")]
        public float FlowBytesPerSecond { get; set; }

        [LoadColumn(15)]
        [ColumnName("FlowPacketsPerSecond")]
        public float FlowPacketsPerSecond { get; set; }

        // Inter-Arrival Time (IAT) Statistics
        [LoadColumn(16)]
        [ColumnName("FlowIATMean")]
        public float FlowIATMean { get; set; }

        [LoadColumn(17)]
        [ColumnName("FlowIATStd")]
        public float FlowIATStd { get; set; }

        [LoadColumn(18)]
        [ColumnName("FlowIATMax")]
        public float FlowIATMax { get; set; }

        [LoadColumn(19)]
        [ColumnName("FlowIATMin")]
        public float FlowIATMin { get; set; }

        [LoadColumn(20)]
        [ColumnName("FwdIATTotal")]
        public float FwdIATTotal { get; set; }

        [LoadColumn(21)]
        [ColumnName("FwdIATMean")]
        public float FwdIATMean { get; set; }

        [LoadColumn(22)]
        [ColumnName("FwdIATStd")]
        public float FwdIATStd { get; set; }

        [LoadColumn(23)]
        [ColumnName("FwdIATMax")]
        public float FwdIATMax { get; set; }

        [LoadColumn(24)]
        [ColumnName("FwdIATMin")]
        public float FwdIATMin { get; set; }

        [LoadColumn(25)]
        [ColumnName("BwdIATTotal")]
        public float BwdIATTotal { get; set; }

        [LoadColumn(26)]
        [ColumnName("BwdIATMean")]
        public float BwdIATMean { get; set; }

        [LoadColumn(27)]
        [ColumnName("BwdIATStd")]
        public float BwdIATStd { get; set; }

        [LoadColumn(28)]
        [ColumnName("BwdIATMax")]
        public float BwdIATMax { get; set; }

        [LoadColumn(29)]
        [ColumnName("BwdIATMin")]
        public float BwdIATMin { get; set; }

        // Flag Counts
        [LoadColumn(30)]
        [ColumnName("FwdPSHFlags")]
        public float FwdPSHFlags { get; set; }

        [LoadColumn(31)]
        [ColumnName("BwdPSHFlags")]
        public float BwdPSHFlags { get; set; }

        [LoadColumn(32)]
        [ColumnName("FwdURGFlags")]
        public float FwdURGFlags { get; set; }

        [LoadColumn(33)]
        [ColumnName("BwdURGFlags")]
        public float BwdURGFlags { get; set; }

        [LoadColumn(34)]
        [ColumnName("FwdHeaderLength")]
        public float FwdHeaderLength { get; set; }

        [LoadColumn(35)]
        [ColumnName("BwdHeaderLength")]
        public float BwdHeaderLength { get; set; }

        [LoadColumn(36)]
        [ColumnName("FwdPacketsPerSecond")]
        public float FwdPacketsPerSecond { get; set; }

        [LoadColumn(37)]
        [ColumnName("BwdPacketsPerSecond")]
        public float BwdPacketsPerSecond { get; set; }

        [LoadColumn(38)]
        [ColumnName("MinPacketLength")]
        public float MinPacketLength { get; set; }

        [LoadColumn(39)]
        [ColumnName("MaxPacketLength")]
        public float MaxPacketLength { get; set; }

        [LoadColumn(40)]
        [ColumnName("PacketLengthMean")]
        public float PacketLengthMean { get; set; }

        [LoadColumn(41)]
        [ColumnName("PacketLengthStd")]
        public float PacketLengthStd { get; set; }

        [LoadColumn(42)]
        [ColumnName("PacketLengthVariance")]
        public float PacketLengthVariance { get; set; }

        [LoadColumn(43)]
        [ColumnName("FINFlagCount")]
        public float FINFlagCount { get; set; }

        [LoadColumn(44)]
        [ColumnName("SYNFlagCount")]
        public float SYNFlagCount { get; set; }

        [LoadColumn(45)]
        [ColumnName("RSTFlagCount")]
        public float RSTFlagCount { get; set; }

        [LoadColumn(46)]
        [ColumnName("PSHFlagCount")]
        public float PSHFlagCount { get; set; }

        [LoadColumn(47)]
        [ColumnName("ACKFlagCount")]
        public float ACKFlagCount { get; set; }

        [LoadColumn(48)]
        [ColumnName("URGFlagCount")]
        public float URGFlagCount { get; set; }

        [LoadColumn(49)]
        [ColumnName("CWEFlagCount")]
        public float CWEFlagCount { get; set; }

        [LoadColumn(50)]
        [ColumnName("ECEFlagCount")]
        public float ECEFlagCount { get; set; }

        [LoadColumn(51)]
        [ColumnName("DownUpRatio")]
        public float DownUpRatio { get; set; }

        [LoadColumn(52)]
        [ColumnName("AveragePacketSize")]
        public float AveragePacketSize { get; set; }

        [LoadColumn(53)]
        [ColumnName("AvgFwdSegmentSize")]
        public float AvgFwdSegmentSize { get; set; }

        [LoadColumn(54)]
        [ColumnName("AvgBwdSegmentSize")]
        public float AvgBwdSegmentSize { get; set; }

        [LoadColumn(55)]
        [ColumnName("FwdHeaderLengthDuplicate")]
        public float FwdHeaderLengthDuplicate { get; set; }

        [LoadColumn(56)]
        [ColumnName("FwdAvgBytesPerBulk")]
        public float FwdAvgBytesPerBulk { get; set; }

        [LoadColumn(57)]
        [ColumnName("FwdAvgPacketsPerBulk")]
        public float FwdAvgPacketsPerBulk { get; set; }

        [LoadColumn(58)]
        [ColumnName("FwdAvgBulkRate")]
        public float FwdAvgBulkRate { get; set; }

        [LoadColumn(59)]
        [ColumnName("BwdAvgBytesPerBulk")]
        public float BwdAvgBytesPerBulk { get; set; }

        [LoadColumn(60)]
        [ColumnName("BwdAvgPacketsPerBulk")]
        public float BwdAvgPacketsPerBulk { get; set; }

        [LoadColumn(61)]
        [ColumnName("BwdAvgBulkRate")]
        public float BwdAvgBulkRate { get; set; }

        [LoadColumn(62)]
        [ColumnName("SubflowFwdPackets")]
        public float SubflowFwdPackets { get; set; }

        [LoadColumn(63)]
        [ColumnName("SubflowFwdBytes")]
        public float SubflowFwdBytes { get; set; }

        [LoadColumn(64)]
        [ColumnName("SubflowBwdPackets")]
        public float SubflowBwdPackets { get; set; }

        [LoadColumn(65)]
        [ColumnName("SubflowBwdBytes")]
        public float SubflowBwdBytes { get; set; }

        [LoadColumn(66)]
        [ColumnName("InitWinBytesForward")]
        public float InitWinBytesForward { get; set; }

        [LoadColumn(67)]
        [ColumnName("InitWinBytesBackward")]
        public float InitWinBytesBackward { get; set; }

        [LoadColumn(68)]
        [ColumnName("ActDataPktFwd")]
        public float ActDataPktFwd { get; set; }

        [LoadColumn(69)]
        [ColumnName("MinSegSizeForward")]
        public float MinSegSizeForward { get; set; }

        [LoadColumn(70)]
        [ColumnName("ActiveMean")]
        public float ActiveMean { get; set; }

        [LoadColumn(71)]
        [ColumnName("ActiveStd")]
        public float ActiveStd { get; set; }

        [LoadColumn(72)]
        [ColumnName("ActiveMax")]
        public float ActiveMax { get; set; }

        [LoadColumn(73)]
        [ColumnName("ActiveMin")]
        public float ActiveMin { get; set; }

        [LoadColumn(74)]
        [ColumnName("IdleMean")]
        public float IdleMean { get; set; }

        [LoadColumn(75)]
        [ColumnName("IdleStd")]
        public float IdleStd { get; set; }

        [LoadColumn(76)]
        [ColumnName("IdleMax")]
        public float IdleMax { get; set; }

        [LoadColumn(77)]
        [ColumnName("IdleMin")]
        public float IdleMin { get; set; }

        // Label - Attack Type (CSV index 78)
        [LoadColumn(78)]
        [ColumnName("Label")]
        public string Label { get; set; } = string.Empty;
    }
}
