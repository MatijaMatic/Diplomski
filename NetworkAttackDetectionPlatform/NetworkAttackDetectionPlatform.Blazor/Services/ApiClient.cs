using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.Blazor.Services.Dtos;

namespace NetworkAttackDetectionPlatform.Blazor.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _http;

        public ApiClient(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task<List<AttackDetectionDto>> GetAllDetectionsAsync()
        {
            var resp = await _http.GetAsync("api/AttackDetection");
            resp.EnsureSuccessStatusCode();
            var dto = await resp.Content.ReadFromJsonAsync<List<AttackDetectionDto>>();
            return dto ?? new List<AttackDetectionDto>();
        }

        public async Task<AttackDetectionDto?> GetDetectionByIdAsync(Guid id)
        {
            var resp = await _http.GetAsync($"api/AttackDetection/{id}");
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<AttackDetectionDto>();
        }

        public async Task<List<RecommendationDto>> GetRecommendationsByAttackIdAsync(Guid attackId)
        {
            var resp = await _http.GetAsync($"api/Recommendation/by-attack/{attackId}");
            resp.EnsureSuccessStatusCode();
            var dto = await resp.Content.ReadFromJsonAsync<List<RecommendationDto>>();
            return dto ?? new List<RecommendationDto>();
        }

        public async Task<AttackDetectionDto?> CreateAttackDetectionAsync(CreateAttackDetectionDto dto)
        {
            var resp = await _http.PostAsJsonAsync("api/AttackDetection", dto);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<AttackDetectionDto>();
        }

        public async Task<RecommendationDto?> CreateRecommendationAsync(CreateRecommendationDto dto)
        {
            var resp = await _http.PostAsJsonAsync("api/Recommendation", dto);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<RecommendationDto>();
        }

        public async Task<bool> UpdateDetectionStatusAsync(Guid id, string status)
        {
            var payload = new StatusUpdateDto { Status = status };
            var resp = await _http.PutAsJsonAsync($"api/AttackDetection/{id}/status", payload);
            if (resp.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<AttackDetectionDto?> PredictAsync(NetworkTrafficDto dto)
        {
            var resp = await _http.PostAsJsonAsync("api/Prediction/predict", dto);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<AttackDetectionDto>();
        }
    }
}
