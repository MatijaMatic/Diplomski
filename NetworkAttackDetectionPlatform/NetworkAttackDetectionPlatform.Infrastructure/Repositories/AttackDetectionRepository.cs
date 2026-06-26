using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetworkAttackDetectionPlatform.Domain.Entities;
using NetworkAttackDetectionPlatform.Domain.Interfaces;
using NetworkAttackDetectionPlatform.Infrastructure.Data;

namespace NetworkAttackDetectionPlatform.Infrastructure.Repositories
{
    public class AttackDetectionRepository : IAttackDetectionRepository
    {
        private readonly ApplicationDbContext _context;

        public AttackDetectionRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Add(AttackDetection entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.AttackDetections.Add(entity);
            _context.SaveChanges();
        }

        public AttackDetection? GetById(Guid id)
        {
            return _context.AttackDetections
                .Include(a => a.Recommendations)
                .SingleOrDefault(a => a.Id == id);
        }

        public IEnumerable<AttackDetection> GetAll()
        {
            return _context.AttackDetections
                .Include(a => a.Recommendations)
                .AsNoTracking()
                .ToList();
        }

        public void Update(AttackDetection entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.AttackDetections.Update(entity);
            _context.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var entity = _context.AttackDetections.Find(id);
            if (entity == null) return;
            _context.AttackDetections.Remove(entity);
            _context.SaveChanges();
        }

        public (IList<AttackDetection> Items, int TotalCount) GetDetections(int pageNumber, int pageSize, int? attackType = null, int? severity = null, int? status = null, DateTime? startDate = null, DateTime? endDate = null, double? minConfidence = null, double? maxConfidence = null)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 50;

            var query = _context.AttackDetections
                .Include(a => a.Recommendations)
                .AsQueryable();

            if (attackType.HasValue)
                query = query.Where(a => (int)a.AttackType == attackType.Value);

            if (severity.HasValue)
                query = query.Where(a => (int)a.Severity == severity.Value);

            if (status.HasValue)
                query = query.Where(a => (int)a.Status == status.Value);

            if (startDate.HasValue)
                query = query.Where(a => a.Occurrence.End >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Occurrence.Start <= endDate.Value);

            if (minConfidence.HasValue)
                query = query.Where(a => a.Confidence.Value >= minConfidence.Value);

            if (maxConfidence.HasValue)
                query = query.Where(a => a.Confidence.Value <= maxConfidence.Value);

            var total = query.Count();
            var items = query.OrderByDescending(a => a.Occurrence.End)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToList();

            return (items, total);
        }
    }
}
