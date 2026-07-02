using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.Blazor.Services.Dtos;

namespace NetworkAttackDetectionPlatform.Blazor.Services
{
    public interface IApiClient
    {
        Task<PagedResult<AttackDetectionDto>> GetAllDetectionsAsync();
        Task<AttackDetectionDto?> GetDetectionByIdAsync(Guid id);
        Task<List<RecommendationDto>> GetRecommendationsByAttackIdAsync(Guid attackId);
        Task<AttackDetectionDto?> CreateAttackDetectionAsync(CreateAttackDetectionDto dto);
        Task<RecommendationDto?> CreateRecommendationAsync(CreateRecommendationDto dto);
        Task<bool> UpdateDetectionStatusAsync(Guid id, string status);
        Task<bool> DeleteDetectionAsync(Guid id);
        Task<AttackDetectionDto?> PredictAsync(NetworkTrafficDto dto);
    }
}
