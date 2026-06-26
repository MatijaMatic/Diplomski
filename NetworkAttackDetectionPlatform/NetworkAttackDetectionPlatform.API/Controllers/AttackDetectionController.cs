using Microsoft.AspNetCore.Mvc;
using NetworkAttackDetectionPlatform.Application.DTOs;
using NetworkAttackDetectionPlatform.Application.DTOs.Filters;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using System;

namespace NetworkAttackDetectionPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttackDetectionController : ControllerBase
    {
        private readonly IAttackDetectionService _service;

        public AttackDetectionController(IAttackDetectionService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Retrieves a paged list of attack detections with optional filters.
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="attackType">Optional attack type filter</param>
        /// <param name="severity">Optional severity filter</param>
        /// <param name="startDate">Optional occurrence start date filter</param>
        /// <param name="endDate">Optional occurrence end date filter</param>
        /// <param name="minConfidence">Optional minimum confidence</param>
        /// <param name="maxConfidence">Optional maximum confidence</param>
        /// <returns>Paged result of AttackDetectionDto</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedResult<AttackDetectionDto>), 200)]
        public IActionResult GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] int? attackType = null,
            [FromQuery] int? severity = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] double? minConfidence = null,
            [FromQuery] double? maxConfidence = null)
        {
            var result = _service.GetDetections(pageNumber, pageSize, attackType, severity, startDate, endDate, minConfidence, maxConfidence);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific attack detection by id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(AttackDetectionDto), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetById(Guid id)
        {
            var item = _service.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Creates a new attack detection.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AttackDetectionDto), 201)]
        [ProducesResponseType(400)]
        public IActionResult Create([FromBody] CreateAttackDetectionDto dto)
        {
            if (dto == null) return BadRequest();
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Deletes an attack detection by id.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        public IActionResult Delete(Guid id)
        {
            _service.Remove(id);
            return NoContent();
        }

        /// <summary>
        /// Updates the status of an attack detection.
        /// </summary>
        [HttpPut("{id:guid}/status")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateStatus(Guid id, [FromBody] StatusUpdateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Status)) return BadRequest();

            try
            {
                _service.SetStatus(id, dto.Status);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            return NoContent();
        }
    }
}
