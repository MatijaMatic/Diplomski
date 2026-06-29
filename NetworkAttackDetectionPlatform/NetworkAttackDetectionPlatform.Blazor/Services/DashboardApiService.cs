using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.Blazor.Services.Dtos;

namespace NetworkAttackDetectionPlatform.Blazor.Services
{
    public class DashboardApiService : IDashboardApiService
    {
        private readonly HttpClient _http;

        public DashboardApiService(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task<DashboardStatisticsDto?> GetStatisticsAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/Dashboard/statistics");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<DashboardStatisticsDto>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<RecentAttackDto>> GetRecentAttacksAsync(int count = 10)
        {
            try
            {
                var response = await _http.GetAsync($"api/Dashboard/recent?count={count}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<RecentAttackDto>>();
                return result ?? new List<RecentAttackDto>();
            }
            catch
            {
                return new List<RecentAttackDto>();
            }
        }

        public async Task<List<AttackTimelineDto>> GetTimelineAsync(int days = 7)
        {
            try
            {
                var response = await _http.GetAsync($"api/Dashboard/timeline?days={days}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<AttackTimelineDto>>();
                return result ?? new List<AttackTimelineDto>();
            }
            catch
            {
                return new List<AttackTimelineDto>();
            }
        }

        public async Task<List<AttackDistributionDto>> GetAttackDistributionAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/Dashboard/attack-distribution");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<AttackDistributionDto>>();
                return result ?? new List<AttackDistributionDto>();
            }
            catch
            {
                return new List<AttackDistributionDto>();
            }
        }

        public async Task<SeverityDistributionDto?> GetSeverityDistributionAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/Dashboard/severity-distribution");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<SeverityDistributionDto>();
            }
            catch
            {
                return null;
            }
        }
    }
}
