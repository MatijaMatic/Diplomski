using Microsoft.ML.Data;

namespace NetworkAttackDetectionPlatform.MachineLearning.Models
{
    /// <summary>
    /// Represents the complete CICIDS2017 dataset feature set for training.
    /// Includes 78 network traffic features plus attack label.
    /// Compatible with ML.NET IDataView for training pipeline integration.
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
        [ColumnName("FINFlagCount")]
        public float FINFlagCount { get; set; }

        [LoadColumn(35)]
        [ColumnName("SYNFlagCount")]
        public float SYNFlagCount { get; set; }

        [LoadColumn(36)]
        [ColumnName("RSTFlagCount")]
        public float RSTFlagCount { get; set; }

        [LoadColumn(37)]
        [ColumnName("PSHFlagCount")]
        public float PSHFlagCount { get; set; }

        [LoadColumn(38)]
        [ColumnName("ACKFlagCount")]
        public float ACKFlagCount { get; set; }

        [LoadColumn(39)]
        [ColumnName("URGFlagCount")]
        public float URGFlagCount { get; set; }

        [LoadColumn(40)]
        [ColumnName("CWEFlagCount")]
        public float CWEFlagCount { get; set; }

        [LoadColumn(41)]
        [ColumnName("ECEFlagCount")]
        public float ECEFlagCount { get; set; }

        // Header Length
        [LoadColumn(42)]
        [ColumnName("FwdHeaderLength")]
        public float FwdHeaderLength { get; set; }

        [LoadColumn(43)]
        [ColumnName("BwdHeaderLength")]
        public float BwdHeaderLength { get; set; }

        // Packets Per Second
        [LoadColumn(44)]
        [ColumnName("FwdPacketsPerSecond")]
        public float FwdPacketsPerSecond { get; set; }

        [LoadColumn(45)]
        [ColumnName("BwdPacketsPerSecond")]
        public float BwdPacketsPerSecond { get; set; }

        // Packet Length Statistics
        [LoadColumn(46)]
        [ColumnName("MinPacketLength")]
        public float MinPacketLength { get; set; }

        [LoadColumn(47)]
        [ColumnName("MaxPacketLength")]
        public float MaxPacketLength { get; set; }

        [LoadColumn(48)]
        [ColumnName("PacketLengthMean")]
        public float PacketLengthMean { get; set; }

        [LoadColumn(49)]
        [ColumnName("PacketLengthStd")]
        public float PacketLengthStd { get; set; }

        [LoadColumn(50)]
        [ColumnName("PacketLengthVariance")]
        public float PacketLengthVariance { get; set; }

        // Down/Up Ratio
        [LoadColumn(51)]
        [ColumnName("DownUpRatio")]
        public float DownUpRatio { get; set; }

        // Average Packet Size
        [LoadColumn(52)]
        [ColumnName("AveragePacketSize")]
        public float AveragePacketSize { get; set; }

        [LoadColumn(53)]
        [ColumnName("AvgFwdSegmentSize")]
        public float AvgFwdSegmentSize { get; set; }

        [LoadColumn(54)]
        [ColumnName("AvgBwdSegmentSize")]
        public float AvgBwdSegmentSize { get; set; }

        // Bulk Features
        [LoadColumn(55)]
        [ColumnName("FwdAvgBytesPerBulk")]
        public float FwdAvgBytesPerBulk { get; set; }

        [LoadColumn(56)]
        [ColumnName("FwdAvgPacketsPerBulk")]
        public float FwdAvgPacketsPerBulk { get; set; }

        [LoadColumn(57)]
        [ColumnName("FwdAvgBulkRate")]
        public float FwdAvgBulkRate { get; set; }

        [LoadColumn(58)]
        [ColumnName("BwdAvgBytesPerBulk")]
        public float BwdAvgBytesPerBulk { get; set; }

        [LoadColumn(59)]
        [ColumnName("BwdAvgPacketsPerBulk")]
        public float BwdAvgPacketsPerBulk { get; set; }

        [LoadColumn(60)]
        [ColumnName("BwdAvgBulkRate")]
        public float BwdAvgBulkRate { get; set; }

        // Subflow Features
        [LoadColumn(61)]
        [ColumnName("SubflowFwdPackets")]
        public float SubflowFwdPackets { get; set; }

        [LoadColumn(62)]
        [ColumnName("SubflowFwdBytes")]
        public float SubflowFwdBytes { get; set; }

        [LoadColumn(63)]
        [ColumnName("SubflowBwdPackets")]
        public float SubflowBwdPackets { get; set; }

        [LoadColumn(64)]
        [ColumnName("SubflowBwdBytes")]
        public float SubflowBwdBytes { get; set; }

        // Init Window Bytes
        [LoadColumn(65)]
        [ColumnName("InitWinBytesForward")]
        public float InitWinBytesForward { get; set; }

        [LoadColumn(66)]
        [ColumnName("InitWinBytesBackward")]
        public float InitWinBytesBackward { get; set; }

        // Active/Idle Time Statistics
        [LoadColumn(67)]
        [ColumnName("ActiveMean")]
        public float ActiveMean { get; set; }

        [LoadColumn(68)]
        [ColumnName("ActiveStd")]
        public float ActiveStd { get; set; }

        [LoadColumn(69)]
        [ColumnName("ActiveMax")]
        public float ActiveMax { get; set; }

        [LoadColumn(70)]
        [ColumnName("ActiveMin")]
        public float ActiveMin { get; set; }

        [LoadColumn(71)]
        [ColumnName("IdleMean")]
        public float IdleMean { get; set; }

        [LoadColumn(72)]
        [ColumnName("IdleStd")]
        public float IdleStd { get; set; }

        [LoadColumn(73)]
        [ColumnName("IdleMax")]
        public float IdleMax { get; set; }

        [LoadColumn(74)]
        [ColumnName("IdleMin")]
        public float IdleMin { get; set; }

        // Additional Features
        [LoadColumn(75)]
        [ColumnName("ActDataPktFwd")]
        public float ActDataPktFwd { get; set; }

        [LoadColumn(76)]
        [ColumnName("MinSegSizeForward")]
        public float MinSegSizeForward { get; set; }

        // Protocol (TCP=6, UDP=17, etc.)
        [LoadColumn(77)]
        [ColumnName("Protocol")]
        public float Protocol { get; set; }

        // Label - Attack Type
        [LoadColumn(78)]
        [ColumnName("Label")]
        public string Label { get; set; } = string.Empty;
    }
}
