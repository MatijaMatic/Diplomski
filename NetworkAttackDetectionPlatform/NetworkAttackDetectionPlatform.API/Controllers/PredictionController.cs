using Microsoft.AspNetCore.Mvc;
using NetworkAttackDetectionPlatform.Application.DTOs.ML;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using System.Reflection;

namespace NetworkAttackDetectionPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly NetworkAttackDetectionPlatform.Application.Interfaces.IAttackPredictionService _predictionService;
        private readonly NetworkAttackDetectionPlatform.Application.Interfaces.IAttackDetectionService _detectionService;

        public PredictionController(IAttackPredictionService predictionService, IAttackDetectionService detectionService)
        {
            _predictionService = predictionService;
            _detectionService = detectionService;
        }

        /// <summary>
        /// [DEPRECATED] 7-feature simplified prediction for demo/testing only.
        /// This endpoint does NOT use a full 78-feature CICIDS2017 model.
        /// Results are NOT scientifically valid.
        /// Use POST /api/prediction/predict-cicids2017 for accurate predictions.
        /// </summary>
        [HttpPost("predict")]
        [Obsolete("Use /api/prediction/predict-cicids2017 for scientifically valid predictions")]
        public async Task<ActionResult<NetworkAttackDetectionPlatform.Application.DTOs.AttackDetectionDto>> Predict([FromBody] NetworkTrafficDto traffic)
        {
            if (traffic == null) return BadRequest("Traffic data is required");

            try
            {
                // Use the configured prediction provider (demo logic)
                var classification = await _predictionService.PredictAsync(traffic).ConfigureAwait(false);

                // Map classification result into CreateAttackDetectionDto
                var createDto = new NetworkAttackDetectionPlatform.Application.DTOs.CreateAttackDetectionDto
                {
                    SourceIp = traffic.SourceIp,
                    DestinationIp = traffic.DestinationIp,
                    SourcePort = traffic.SourcePort,
                    DestinationPort = traffic.DestinationPort,
                    Protocol = traffic.Protocol,
                    AttackType = classification?.AttackType ?? 0,
                    Severity = classification?.Severity ?? 0,
                    Confidence = classification?.Confidence ?? 0.0,
                    OccurrenceStart = traffic.Timestamp,
                    OccurrenceEnd = traffic.Timestamp
                };

                var persisted = _detectionService.Create(createDto);

                return Ok(persisted);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Prediction failed: {ex.Message}");
            }
        }

        /// <summary>
        /// [PREFERRED] Full 78-feature CICIDS2017 network traffic prediction.
        /// Requires all 78 CICIDS2017 numerical features.
        /// Uses the trained ML model for accurate attack classification.
        /// Returns scientifically valid results suitable for thesis documentation.
        /// </summary>
        [HttpPost("predict-cicids2017")]
        public async Task<ActionResult<NetworkAttackDetectionPlatform.Application.DTOs.AttackDetectionDto>> PredictCicids2017([FromBody] Cicids2017PredictionRequest traffic)
        {
            if (traffic == null)
                return BadRequest("CICIDS2017 prediction request is required");

            try
            {
                // Validate all 78 features are present
                var missingFeatures = ValidateCicids2017Request(traffic);
                if (missingFeatures.Count > 0)
                {
                    return BadRequest(new
                    {
                        error = "Invalid CICIDS2017 prediction request",
                        message = $"Missing or invalid features: {string.Join(", ", missingFeatures)}",
                        required_features_count = 78,
                        hint = "All 78 CICIDS2017 numerical features must be provided with valid numeric values"
                    });
                }

                // Use the 78-feature prediction (real ML model)
                var classification = await _predictionService.PredictAsync(traffic).ConfigureAwait(false);

                if (!classification.IsMLBacked)
                {
                    return StatusCode(503, new
                    {
                        error = "ML model unavailable",
                        message = "Trained ML model not loaded. Cannot provide scientifically valid predictions.",
                        fallback_used = true
                    });
                }

                // Map classification result into CreateAttackDetectionDto
                var createDto = new NetworkAttackDetectionPlatform.Application.DTOs.CreateAttackDetectionDto
                {
                    SourceIp = traffic.SourceIp,
                    DestinationIp = traffic.DestinationIp,
                    SourcePort = (int)traffic.DestinationPort,  // Using DestinationPort as source for mapping
                    DestinationPort = (int)traffic.DestinationPort,
                    Protocol = traffic.Protocol,
                    AttackType = classification?.AttackType ?? 0,
                    Severity = classification?.Severity ?? 0,
                    Confidence = classification?.Confidence ?? 0.0,
                    OccurrenceStart = System.DateTime.UtcNow,
                    OccurrenceEnd = System.DateTime.UtcNow
                };

                var persisted = _detectionService.Create(createDto);

                return Ok(persisted);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = "Invalid prediction request",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Prediction failed",
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Validates that all required CICIDS2017 features are present in the request.
        /// </summary>
        private static System.Collections.Generic.List<string> ValidateCicids2017Request(Cicids2017PredictionRequest request)
        {
            var errors = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(request.SourceIp))
                errors.Add("SourceIp");

            if (string.IsNullOrWhiteSpace(request.DestinationIp))
                errors.Add("DestinationIp");

            // Check all float properties
            var properties = typeof(Cicids2017PredictionRequest).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
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
