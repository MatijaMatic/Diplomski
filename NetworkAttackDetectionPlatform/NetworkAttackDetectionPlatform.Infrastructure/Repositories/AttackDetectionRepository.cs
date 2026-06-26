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
    }
}
