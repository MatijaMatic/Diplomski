using System;
using System.Collections.Generic;
using NetworkAttackDetectionPlatform.Application.DTOs;

namespace NetworkAttackDetectionPlatform.Application.Interfaces
{
    public interface IAttackDetectionService
    {
        IEnumerable<AttackDetectionDto> GetAll();

        AttackDetectionDto? GetById(Guid id);

        AttackDetectionDto Create(CreateAttackDetectionDto dto);

        void AddRecommendation(Guid attackDetectionId, string text);

        void Remove(Guid id);

        void Update(AttackDetectionDto dto);

        // New: filtered and paged retrieval
        PagedResult<AttackDetectionDto> GetDetections(
            int pageNumber,
            int pageSize,
            int? attackType = null,
            int? severity = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            double? minConfidence = null,
            double? maxConfidence = null
        );

        // New: set status by name
        void SetStatus(Guid id, string statusName);
    }
}