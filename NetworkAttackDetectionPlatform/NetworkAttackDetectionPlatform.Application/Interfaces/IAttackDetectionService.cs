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
    }
}