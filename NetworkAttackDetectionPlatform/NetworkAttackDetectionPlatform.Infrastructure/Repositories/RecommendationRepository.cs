using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetworkAttackDetectionPlatform.Domain.Entities;
using NetworkAttackDetectionPlatform.Domain.Interfaces;
using NetworkAttackDetectionPlatform.Infrastructure.Data;

namespace NetworkAttackDetectionPlatform.Infrastructure.Repositories
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly ApplicationDbContext _context;

        public RecommendationRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Add(Recommendation recommendation)
        {
            if (recommendation == null) throw new ArgumentNullException(nameof(recommendation));
            _context.Recommendations.Add(recommendation);
            _context.SaveChanges();
        }

        public Recommendation? GetById(Guid id)
        {
            return _context.Recommendations.Find(id);
        }

        public IEnumerable<Recommendation> GetByAttackDetectionId(Guid attackDetectionId)
        {
            return _context.Recommendations
                .Where(r => r.AttackDetectionId == attackDetectionId)
                .AsNoTracking()
                .ToList();
        }

        public void Update(Recommendation recommendation)
        {
            if (recommendation == null) throw new ArgumentNullException(nameof(recommendation));
            _context.Recommendations.Update(recommendation);
            _context.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var rec = _context.Recommendations.Find(id);
            if (rec == null) return;
            _context.Recommendations.Remove(rec);
            _context.SaveChanges();
        }
    }
}
