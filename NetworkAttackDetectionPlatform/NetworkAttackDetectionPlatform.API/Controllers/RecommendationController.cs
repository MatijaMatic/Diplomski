using Microsoft.AspNetCore.Mvc;
using NetworkAttackDetectionPlatform.Application.DTOs;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using System;

namespace NetworkAttackDetectionPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _service;

        public RecommendationController(IRecommendationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("by-attack/{attackId:guid}")]
        public IActionResult GetByAttack(Guid attackId)
        {
            var items = _service.GetByAttackDetectionId(attackId);
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            var item = _service.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateRecommendationDto dto)
        {
            if (dto == null) return BadRequest();
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
