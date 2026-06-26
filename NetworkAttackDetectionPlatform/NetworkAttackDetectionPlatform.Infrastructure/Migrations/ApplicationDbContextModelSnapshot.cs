using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace NetworkAttackDetectionPlatform.Infrastructure.Migrations
{
    [DbContext(typeof(NetworkAttackDetectionPlatform.Infrastructure.Data.ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("NetworkAttackDetectionPlatform.Domain.Entities.AttackDetection", b =>
            {
                b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");

                b.Property<string>("SourceIp").IsRequired().HasColumnType("nvarchar(max)");

                b.Property<string>("DestinationIp").IsRequired().HasColumnType("nvarchar(max)");

                b.Property<int>("SourcePort").HasColumnType("int");

                b.Property<int>("DestinationPort").HasColumnType("int");

                b.Property<int>("Protocol").HasColumnType("int");

                b.Property<int>("AttackType").HasColumnType("int");

                b.Property<int>("Severity").HasColumnType("int");

                b.Property<double>("Confidence").HasColumnType("float");

                b.Property<DateTime>("OccurrenceStart").HasColumnType("datetime2");

                b.Property<DateTime>("OccurrenceEnd").HasColumnType("datetime2");

                b.HasKey("Id");

                b.ToTable("AttackDetections");
            });

            modelBuilder.Entity("NetworkAttackDetectionPlatform.Domain.Entities.Recommendation", b =>
            {
                b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");

                b.Property<Guid>("AttackDetectionId").HasColumnType("uniqueidentifier");

                b.Property<string>("Text").IsRequired().HasColumnType("nvarchar(max)");

                b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");

                b.HasKey("Id");

                b.HasIndex("AttackDetectionId");

                b.ToTable("Recommendations");
            });

            modelBuilder.Entity("NetworkAttackDetectionPlatform.Domain.Entities.Recommendation", b =>
            {
                b.HasOne("NetworkAttackDetectionPlatform.Domain.Entities.AttackDetection")
                    .WithMany("Recommendations")
                    .HasForeignKey("AttackDetectionId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
        }
    }
}
