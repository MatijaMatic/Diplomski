using Microsoft.AspNetCore.Mvc;
using NetworkAttackDetectionPlatform.Application.DTOs.ML;
using NetworkAttackDetectionPlatform.Application.Interfaces;

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

        [HttpPost("predict")]
        public async Task<ActionResult<NetworkAttackDetectionPlatform.Application.DTOs.AttackDetectionDto>> Predict([FromBody] NetworkTrafficDto traffic)
        {
            if (traffic == null) return BadRequest();

            // Use the configured prediction provider (MachineLearning integration)
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
    }
}
