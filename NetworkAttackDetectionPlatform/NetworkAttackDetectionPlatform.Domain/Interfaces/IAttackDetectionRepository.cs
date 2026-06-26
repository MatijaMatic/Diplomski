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

        // Query with filtering and paging executed in infrastructure (EF Core)        (IList<AttackDetection> Items, int TotalCount) GetDetections(            int pageNumber,            int pageSize,            int? attackType = null,            int? severity = null,            int? status = null,            DateTime? startDate = null,            DateTime? endDate = null,            double? minConfidence = null,            double? maxConfidence = null        );    }
}
