using Microsoft.AspNetCore.Mvc;
using NetworkAttackDetectionPlatform.Application.DTOs;
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

        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _service.GetAll();
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
        public IActionResult Create([FromBody] CreateAttackDetectionDto dto)
        {
            if (dto == null) return BadRequest();
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            _service.Remove(id);
            return NoContent();
        }
    }
}
