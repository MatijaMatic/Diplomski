using Microsoft.EntityFrameworkCore;
using NetworkAttackDetectionPlatform.Domain.Entities;
using NetworkAttackDetectionPlatform.Infrastructure.Configurations;

namespace NetworkAttackDetectionPlatform.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AttackDetection> AttackDetections { get; set; } = null!;

        public DbSet<Recommendation> Recommendations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AttackDetectionConfiguration());
            modelBuilder.ApplyConfiguration(new RecommendationConfiguration());
        }
    }
}