using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetworkAttackDetectionPlatform.Domain.Entities;
using NetworkAttackDetectionPlatform.Domain.ValueObjects;

namespace NetworkAttackDetectionPlatform.Infrastructure.Configurations
{
    internal class AttackDetectionConfiguration : IEntityTypeConfiguration<AttackDetection>
    {
        public void Configure(EntityTypeBuilder<AttackDetection> builder)
        {
            builder.ToTable("AttackDetections");

            builder.HasKey(a => a.Id);

            // Value objects
            builder.OwnsOne(a => a.SourceIp, sa =>
            {
                sa.Property(p => p.Value).HasColumnName("SourceIp").IsRequired();
            });

            builder.OwnsOne(a => a.DestinationIp, da =>
            {
                da.Property(p => p.Value).HasColumnName("DestinationIp").IsRequired();
            });

            builder.OwnsOne(a => a.SourcePort, sp =>
            {
                sp.Property(p => p.Value).HasColumnName("SourcePort").IsRequired();
            });

            builder.OwnsOne(a => a.DestinationPort, dp =>
            {
                dp.Property(p => p.Value).HasColumnName("DestinationPort").IsRequired();
            });

            builder.OwnsOne(a => a.Confidence, c =>
            {
                c.Property(p => p.Value).HasColumnName("Confidence").IsRequired();
            });

            builder.OwnsOne(a => a.Occurrence, tr =>
            {
                tr.Property(p => p.Start).HasColumnName("OccurrenceStart").IsRequired();
                tr.Property(p => p.End).HasColumnName("OccurrenceEnd").IsRequired();
            });

            // Enums as integers
            builder.Property(a => a.Protocol).HasColumnName("Protocol");
            builder.Property(a => a.AttackType).HasColumnName("AttackType");
            builder.Property(a => a.Severity).HasColumnName("Severity");

            // Status and timestamps
            builder.Property<int>("Status").HasColumnName("Status").IsRequired().HasDefaultValue(0);
            builder.Property<DateTime>("CreatedAt").HasColumnName("CreatedAt").IsRequired();
            builder.Property<DateTime>("UpdatedAt").HasColumnName("UpdatedAt").IsRequired();

            // Relationships - configure using backing field
            builder.Metadata.FindNavigation("Recommendations")?.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(a => a.Recommendations)
                .WithOne()
                .HasForeignKey("AttackDetectionId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
