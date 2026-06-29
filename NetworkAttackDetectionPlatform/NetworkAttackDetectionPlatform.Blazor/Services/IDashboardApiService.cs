using System.Collections.Generic;
using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.Blazor.Services.Dtos;

namespace NetworkAttackDetectionPlatform.Blazor.Services
{
    public interface IDashboardApiService
    {
        Task<DashboardStatisticsDto?> GetStatisticsAsync();
        Task<List<RecentAttackDto>> GetRecentAttacksAsync(int count = 10);
        Task<List<AttackTimelineDto>> GetTimelineAsync(int days = 7);
        Task<List<AttackDistributionDto>> GetAttackDistributionAsync();
        Task<SeverityDistributionDto?> GetSeverityDistributionAsync();
    }
}
