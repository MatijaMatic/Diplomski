using System;
using System.Collections.Generic;
using NetworkAttackDetectionPlatform.Domain.Entities;

namespace NetworkAttackDetectionPlatform.Domain.Interfaces
{
    public interface IRecommendationRepository
    {
        void Add(Recommendation recommendation);

        Recommendation? GetById(Guid id);

        IEnumerable<Recommendation> GetByAttackDetectionId(Guid attackDetectionId);

        void Update(Recommendation recommendation);

        void Remove(Guid id);
    }
}
