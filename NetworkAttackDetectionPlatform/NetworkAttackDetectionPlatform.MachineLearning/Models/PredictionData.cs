using Microsoft.ML.Data;

namespace NetworkAttackDetectionPlatform.MachineLearning.Models
{
    /// <summary>
    /// Represents input data for making predictions with a trained ML model.
    /// This class is used during inference to feed data into the model.
    /// </summary>
    public sealed class PredictionData
    {
        public string SourceIp { get; set; } = string.Empty;

        public string DestinationIp { get; set; } = string.Empty;

        public float SourcePort { get; set; }

        public float DestinationPort { get; set; }

        public float Protocol { get; set; }

        public float PayloadSize { get; set; }

        public float PacketCount { get; set; }

        public float Duration { get; set; }

        public float BytesTransferred { get; set; }
    }
}
