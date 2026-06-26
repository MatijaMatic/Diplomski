using System;
using System.Collections.Generic;
using NetworkAttackDetectionPlatform.Application.DTOs;

namespace NetworkAttackDetectionPlatform.Application.Interfaces
{
    public interface IRecommendationService
    {
        IEnumerable<RecommendationDto> GetByAttackDetectionId(Guid attackDetectionId);

        RecommendationDto? GetById(Guid id);

        RecommendationDto Create(CreateRecommendationDto dto);

        void Update(RecommendationDto dto);

        void Remove(Guid id);
    }
}
