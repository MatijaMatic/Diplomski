using System.Collections.Generic;
using NetworkAttackDetectionPlatform.Application.DTOs.Dashboard;

namespace NetworkAttackDetectionPlatform.Application.Interfaces
{
    public interface IDashboardService
    {
        DashboardStatisticsDto GetStatistics();
        IEnumerable<RecentAttackDto> GetRecentAttacks(int count = 10);
        IEnumerable<AttackTimelineDto> GetTimeline(int days = 7);
        IEnumerable<AttackDistributionDto> GetAttackDistribution();
        SeverityDistributionDto GetSeverityDistribution();
    }
}
