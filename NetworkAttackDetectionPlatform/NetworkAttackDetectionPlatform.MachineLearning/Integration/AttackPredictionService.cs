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

            var confidence = pred?.Score ?? 0.0;

            // Map textual label to AttackType enum using utility mapper
            var mappedAttackType = NetworkAttackDetectionPlatform.MachineLearning.Utilities.LabelToAttackTypeMapper.MapLabelToAttackType(pred?.Label ?? string.Empty);

            var severity = NetworkAttackDetectionPlatform.MachineLearning.Utilities.LabelToAttackTypeMapper.MapConfidenceToSeverity(confidence, mappedAttackType);

            var dto = new AttackClassificationResultDto
            {
                Id = System.Guid.NewGuid(),
                AttackType = (int)mappedAttackType,
                Severity = severity,
                Confidence = confidence,
                DetectedAt = System.DateTime.UtcNow
            };

            return dto;
        }
    }
}
