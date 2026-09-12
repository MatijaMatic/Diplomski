using System;
using System.Reflection;
using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.Application.DTOs.ML;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using NetworkAttackDetectionPlatform.MachineLearning.Features;
using NetworkAttackDetectionPlatform.MachineLearning.Prediction;
using NetworkAttackDetectionPlatform.MachineLearning.Preprocessing;
using NetworkAttackDetectionPlatform.MachineLearning.Utilities;

namespace NetworkAttackDetectionPlatform.MachineLearning.Integration
{
    public class AttackPredictionService : IAttackPredictionService
    {
        private readonly IPredictionService _inner;

        public AttackPredictionService(IPredictionService inner)
        {
            _inner = inner;
        }

        /// <summary>
        /// [DEPRECATED] 7-feature prediction for demo/testing only.
        /// This endpoint provides only partial features and is NOT scientifically valid.
        /// </summary>
        public async Task<AttackClassificationResultDto> PredictAsync(NetworkTrafficDto traffic)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[ML] ??  WARNING: 7-feature demo prediction (not scientifically valid)");
            Console.WriteLine($"[ML] ??  Use Cicids2017PredictionRequest for accurate ML-based predictions");
            Console.ResetColor();

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
            var mappedAttackType = LabelToAttackTypeMapper.MapLabelToAttackType(pred?.Label ?? string.Empty);

            var severity = LabelToAttackTypeMapper.MapConfidenceToSeverity(confidence, mappedAttackType);

            var dto = new AttackClassificationResultDto
            {
                Id = Guid.NewGuid(),
                AttackType = (int)mappedAttackType,
                Severity = severity,
                Confidence = confidence,
                DetectedAt = DateTime.UtcNow,
                IsMLBacked = false  // Mark as not real ML
            };

            return dto;
        }

        /// <summary>
        /// [PREFERRED] Full 78-feature CICIDS2017 prediction - scientifically valid.
        /// All 78 network traffic features MUST be provided.
        /// Missing or zero-filled features will result in invalid predictions.
        /// </summary>
        public async Task<AttackClassificationResultDto> PredictAsync(Cicids2017PredictionRequest traffic)
        {
            if (traffic == null)
                throw new ArgumentNullException(nameof(traffic));

            // Validate that all required features are present (not zero-filled)
            var missingFeatures = ValidatePredictionRequest(traffic);
            if (missingFeatures.Count > 0)
            {
                throw new ArgumentException(
                    $"Invalid prediction request: Missing or invalid features: {string.Join(", ", missingFeatures)}. " +
                    $"All 78 CICIDS2017 features are required for valid ML predictions.");
            }

            // Map all 78 features to FeatureVector in correct order
            var fv = new FeatureVector();

            // Use reflection to read all float properties in order
            var properties = typeof(Cicids2017PredictionRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var featureNames = FeatureConfiguration.NumericalFeatures;

            var propIndex = 0;
            foreach (var prop in properties)
            {
                // Skip non-float properties (SourceIp, DestinationIp)
                if (prop.PropertyType != typeof(float))
                    continue;

                if (propIndex < featureNames.Length)
                {
                    var value = prop.GetValue(traffic);
                    if (value != null && float.TryParse(value.ToString(), out var floatValue))
                    {
                        fv.Features[featureNames[propIndex]] = floatValue;
                    }
                }
                propIndex++;
            }

            // Store identifiers for later use
            fv.Features["SourceIp"] = traffic.SourceIp;
            fv.Features["DestinationIp"] = traffic.DestinationIp;

            // Perform actual ML prediction
            var pred = await _inner.PredictAsync(fv).ConfigureAwait(false);

            var confidence = pred?.Score ?? 0.0;

            // Map textual label to AttackType enum
            var mappedAttackType = LabelToAttackTypeMapper.MapLabelToAttackType(pred?.Label ?? string.Empty);

            var severity = LabelToAttackTypeMapper.MapConfidenceToSeverity(confidence, mappedAttackType);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[ML] ? 78-feature CICIDS2017 prediction: {pred?.Label} (confidence: {confidence:F1}%)");
            Console.ResetColor();

            var dto = new AttackClassificationResultDto
            {
                Id = Guid.NewGuid(),
                AttackType = (int)mappedAttackType,
                Severity = severity,
                Confidence = confidence,
                DetectedAt = DateTime.UtcNow,
                IsMLBacked = true  // Mark as real ML prediction
            };

            return dto;
        }

        /// <summary>
        /// Validates that all 78 features are present in the prediction request.
        /// </summary>
        private static System.Collections.Generic.List<string> ValidatePredictionRequest(Cicids2017PredictionRequest request)
        {
            var errors = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(request.SourceIp))
                errors.Add("SourceIp");

            if (string.IsNullOrWhiteSpace(request.DestinationIp))
                errors.Add("DestinationIp");

            // Check all float properties for valid (non-NaN, non-Infinity) values
            var properties = typeof(Cicids2017PredictionRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(float))
                {
                    var value = (float?)prop.GetValue(request);
                    if (!value.HasValue || float.IsNaN(value.Value) || float.IsInfinity(value.Value))
                    {
                        errors.Add(prop.Name);
                    }
                }
            }

            return errors;
        }
    }
}
