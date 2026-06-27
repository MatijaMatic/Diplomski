using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.Application.DTOs.ML;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using NetworkAttackDetectionPlatform.MachineLearning.Features;
using NetworkAttackDetectionPlatform.MachineLearning.Prediction;

namespace NetworkAttackDetectionPlatform.MachineLearning.Integration
{
    public class AttackPredictionService : IAttackPredictionService
    {
        private readonly IPredictionService _inner;

        public AttackPredictionService(IPredictionService inner)
        {
            _inner = inner;
        }

        public async Task<AttackClassificationResultDto> PredictAsync(NetworkTrafficDto traffic)
        {
            // Simple feature mapping - real feature engineering will be more complex
            var fv = new FeatureVector();
            fv.Features["SourceIp"] = traffic.SourceIp;
            fv.Features["DestinationIp"] = traffic.DestinationIp;
            fv.Features["SourcePort"] = traffic.SourcePort;
            fv.Features["DestinationPort"] = traffic.DestinationPort;
            fv.Features["Protocol"] = traffic.Protocol;
            fv.Features["Timestamp"] = traffic.Timestamp;
            fv.Features["PayloadSize"] = traffic.PayloadSize;

            var pred = await _inner.PredictAsync(fv).ConfigureAwait(false);

            // Map PredictionResult to AttackClassificationResultDto. Use placeholders if unknown.
            var dto = new AttackClassificationResultDto
            {
                Id = System.Guid.NewGuid(),
                AttackType = 0,
                Severity = 0,
                Confidence = pred?.Score ?? 0.0,
                DetectedAt = System.DateTime.UtcNow
            };

            // Try to map label if numeric encoded in Label
            if (!string.IsNullOrWhiteSpace(pred?.Label) && int.TryParse(pred.Label, out var t))
            {
                dto.AttackType = t;
            }

            return dto;
        }
    }
}
