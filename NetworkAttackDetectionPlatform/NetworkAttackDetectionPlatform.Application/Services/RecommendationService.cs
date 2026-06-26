using System;
using System.Collections.Generic;
using System.Linq;
using NetworkAttackDetectionPlatform.Application.DTOs;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using NetworkAttackDetectionPlatform.Domain.Entities;
using NetworkAttackDetectionPlatform.Domain.Interfaces;

namespace NetworkAttackDetectionPlatform.Application.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationRepository _repo;

        public RecommendationService(IRecommendationRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public IEnumerable<RecommendationDto> GetByAttackDetectionId(Guid attackDetectionId)
        {
            var items = _repo.GetByAttackDetectionId(attackDetectionId);
            return items.Select(MapToDto).ToList();
        }

        public RecommendationDto? GetById(Guid id)
        {
            var item = _repo.GetById(id);
            return item == null ? null : MapToDto(item);
        }

        public RecommendationDto Create(CreateRecommendationDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var rec = Recommendation.Create(dto.AttackDetectionId, dto.Text);
            _repo.Add(rec);
            return MapToDto(rec);
        }

        public void Update(RecommendationDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existing = _repo.GetById(dto.Id) ?? throw new InvalidOperationException("Recommendation not found.");

            // Only text can be updated
            // Use reflection of existing domain object: there's no setter except private; but Recommendation has SetText private. So to update, recreate? Domain rules require controlled creation; but repository Update expects full entity.
            // We'll construct a new Recommendation instance via reflection is not allowed. Instead, throw if change not allowed.

            if (existing.Text != dto.Text)
            {
                // Domain Recommendation does not expose an update method for text; recreate not desired. Throw to indicate operation unsupported.
                throw new InvalidOperationException("Updating recommendation text is not supported by domain model.");
            }

            // No changes to persist
        }

        public void Remove(Guid id)
        {
            _repo.Remove(id);
        }

        private static RecommendationDto MapToDto(Recommendation src)
        {
            return new RecommendationDto
            {
                Id = src.Id,
                AttackDetectionId = src.AttackDetectionId,
                Text = src.Text,
                CreatedAt = src.CreatedAt
            };
        }
    }
}
