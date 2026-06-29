using Microsoft.AspNetCore.Mvc;
using NetworkAttackDetectionPlatform.Application.DTOs.Dashboard;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace NetworkAttackDetectionPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Gets dashboard statistics summary.
        /// </summary>
        /// <returns>Dashboard statistics including total attacks, attacks today, critical attacks, average confidence, and most common attack type.</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(DashboardStatisticsDto), 200)]
        public IActionResult GetStatistics()
        {
            var stats = _service.GetStatistics();
            return Ok(stats);
        }

        /// <summary>
        /// Gets recent attack detections.
        /// </summary>
        /// <param name="count">Number of recent attacks to retrieve (default: 10)</param>
        /// <returns>List of recent attacks ordered by newest first.</returns>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(IEnumerable<RecentAttackDto>), 200)]
        public IActionResult GetRecentAttacks([FromQuery] int count = 10)
        {
            var recent = _service.GetRecentAttacks(count);
            return Ok(recent);
        }

        /// <summary>
        /// Gets attack timeline data for the specified number of days.
        /// </summary>
        /// <param name="days">Number of days to include in timeline (default: 7)</param>
        /// <returns>Daily attack counts suitable for charting.</returns>
        [HttpGet("timeline")]
        [ProducesResponseType(typeof(IEnumerable<AttackTimelineDto>), 200)]
        public IActionResult GetTimeline([FromQuery] int days = 7)
        {
            var timeline = _service.GetTimeline(days);
            return Ok(timeline);
        }

        /// <summary>
        /// Gets attack type distribution statistics.
        /// </summary>
        /// <returns>Count of attacks by attack type.</returns>
        [HttpGet("attack-distribution")]
        [ProducesResponseType(typeof(IEnumerable<AttackDistributionDto>), 200)]
        public IActionResult GetAttackDistribution()
        {
            var distribution = _service.GetAttackDistribution();
            return Ok(distribution);
        }

        /// <summary>
        /// Gets severity level distribution statistics.
        /// </summary>
        /// <returns>Count of attacks by severity level (Critical, High, Medium, Low, Unknown).</returns>
        [HttpGet("severity-distribution")]
        [ProducesResponseType(typeof(SeverityDistributionDto), 200)]
        public IActionResult GetSeverityDistribution()
        {
            var distribution = _service.GetSeverityDistribution();
            return Ok(distribution);
        }
    }
}
