using System;
using System.Collections.Generic;
using NetworkAttackDetectionPlatform.Domain.Entities;

namespace NetworkAttackDetectionPlatform.Domain.Interfaces
{
    public interface IAttackDetectionRepository
    {
        void Add(AttackDetection entity);

        AttackDetection? GetById(Guid id);

        IEnumerable<AttackDetection> GetAll();

        void Update(AttackDetection entity);

        void Remove(Guid id);
    }
}
