using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetworkAttackDetectionPlatform.Domain.Entities;

namespace NetworkAttackDetectionPlatform.Infrastructure.Configurations
{
    internal class RecommendationConfiguration : IEntityTypeConfiguration<Recommendation>
    {
        public void Configure(EntityTypeBuilder<Recommendation> builder)
        {
            builder.ToTable("Recommendations");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.AttackDetectionId).HasColumnName("AttackDetectionId").IsRequired();
            builder.Property(r => r.Text).HasColumnName("Text").IsRequired();
            builder.Property(r => r.CreatedAt).HasColumnName("CreatedAt").IsRequired();

            builder.HasIndex("AttackDetectionId");
        }
    }
}
