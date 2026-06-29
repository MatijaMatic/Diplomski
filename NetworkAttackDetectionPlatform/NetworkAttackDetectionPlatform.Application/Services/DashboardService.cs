using System;
using System.Collections.Generic;
using System.Linq;
using NetworkAttackDetectionPlatform.Application.DTOs.Dashboard;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using NetworkAttackDetectionPlatform.Domain.Enums;
using NetworkAttackDetectionPlatform.Domain.Interfaces;

namespace NetworkAttackDetectionPlatform.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IAttackDetectionRepository _repository;

        public DashboardService(IAttackDetectionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public DashboardStatisticsDto GetStatistics()
        {
            var allAttacks = _repository.GetAll().ToList();
            var today = DateTime.UtcNow.Date;

            var totalAttacks = allAttacks.Count;
            var attacksToday = allAttacks.Count(a => a.CreatedAt.Date == today);
            var criticalAttacks = allAttacks.Count(a => a.Severity == SeverityLevelEnum.Critical);
            var averageConfidence = allAttacks.Any() ? allAttacks.Average(a => a.Confidence.Value) : 0.0;

            var mostCommonAttackType = allAttacks
                .GroupBy(a => a.AttackType)
                .OrderByDescending(g => g.Count())
                .Select(g => (int)g.Key)
                .FirstOrDefault();

            return new DashboardStatisticsDto
            {
                TotalAttacks = totalAttacks,
                AttacksToday = attacksToday,
                CriticalAttacks = criticalAttacks,
                AverageConfidence = Math.Round(averageConfidence, 2),
                MostCommonAttackType = mostCommonAttackType
            };
        }

        public IEnumerable<RecentAttackDto> GetRecentAttacks(int count = 10)
        {
            if (count <= 0) count = 10;

            var attacks = _repository.GetAll()
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .Select(a => new RecentAttackDto
                {
                    Id = a.Id,
                    SourceIp = a.SourceIp.Value,
                    DestinationIp = a.DestinationIp.Value,
                    AttackType = (int)a.AttackType,
                    Severity = (int)a.Severity,
                    Confidence = a.Confidence.Value,
                    Status = (int)a.Status,
                    Timestamp = a.CreatedAt
                })
                .ToList();

            return attacks;
        }

        public IEnumerable<AttackTimelineDto> GetTimeline(int days = 7)
        {
            if (days <= 0) days = 7;

            var startDate = DateTime.UtcNow.Date.AddDays(-days + 1);
            var attacks = _repository.GetAll()
                .Where(a => a.CreatedAt.Date >= startDate)
                .ToList();

            var timeline = attacks
                .GroupBy(a => a.CreatedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new AttackTimelineDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count()
                })
                .ToList();

            // Fill in missing dates with zero counts
            var result = new List<AttackTimelineDto>();
            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                var existing = timeline.FirstOrDefault(t => t.Date == date.ToString("yyyy-MM-dd"));
                result.Add(existing ?? new AttackTimelineDto
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    Count = 0
                });
            }

            return result;
        }

        public IEnumerable<AttackDistributionDto> GetAttackDistribution()
        {
            var attacks = _repository.GetAll().ToList();

            var distribution = attacks
                .GroupBy(a => a.AttackType)
                .Select(g => new AttackDistributionDto
                {
                    AttackType = g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderByDescending(d => d.Count)
                .ToList();

            return distribution;
        }

        public SeverityDistributionDto GetSeverityDistribution()
        {
            var attacks = _repository.GetAll().ToList();

            return new SeverityDistributionDto
            {
                Critical = attacks.Count(a => a.Severity == SeverityLevelEnum.Critical),
                High = attacks.Count(a => a.Severity == SeverityLevelEnum.High),
                Medium = attacks.Count(a => a.Severity == SeverityLevelEnum.Medium),
                Low = attacks.Count(a => a.Severity == SeverityLevelEnum.Low),
                Unknown = attacks.Count(a => a.Severity == SeverityLevelEnum.Unknown)
            };
        }
    }
}
