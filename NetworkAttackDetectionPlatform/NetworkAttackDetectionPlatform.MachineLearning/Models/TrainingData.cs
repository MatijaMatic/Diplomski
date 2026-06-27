using Microsoft.ML.Data;

namespace NetworkAttackDetectionPlatform.MachineLearning.Models
{
    /// <summary>
    /// Represents input data for training the ML model.
    /// This class is used during the training phase to load data from datasets.
    /// </summary>
    public sealed class TrainingData
    {
        [LoadColumn(0)]
        public string SourceIp { get; set; } = string.Empty;

        [LoadColumn(1)]
        public string DestinationIp { get; set; } = string.Empty;

        [LoadColumn(2)]
        public float SourcePort { get; set; }

        [LoadColumn(3)]
        public float DestinationPort { get; set; }

        [LoadColumn(4)]
        public float Protocol { get; set; }

        [LoadColumn(5)]
        public float PayloadSize { get; set; }

        [LoadColumn(6)]
        public float PacketCount { get; set; }

        [LoadColumn(7)]
        public float Duration { get; set; }

        [LoadColumn(8)]
        public float BytesTransferred { get; set; }

        // Label column - attack type as string (e.g., "Normal", "DDoS", "PortScan")
        [LoadColumn(9)]
        public string Label { get; set; } = string.Empty;
    }
}
