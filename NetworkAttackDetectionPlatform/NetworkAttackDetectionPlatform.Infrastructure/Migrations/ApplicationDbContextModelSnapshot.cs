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
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<int>("AttackType")
                    .HasColumnType("int")
                    .HasColumnName("AttackType");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("CreatedAt");

                b.Property<int>("Protocol")
                    .HasColumnType("int")
                    .HasColumnName("Protocol");

                b.Property<int>("Severity")
                    .HasColumnType("int")
                    .HasColumnName("Severity");

                b.Property<int>("Status")
                    .HasColumnType("int")
                    .HasColumnName("Status");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("UpdatedAt");

                b.HasKey("Id");

                b.ToTable("AttackDetections", (string)null);

                b.OwnsOne("NetworkAttackDetectionPlatform.Domain.ValueObjects.ConfidenceScore", "Confidence", b1 =>
                {
                    b1.Property<Guid>("AttackDetectionId")
                        .HasColumnType("uniqueidentifier");

                    b1.Property<double>("Value")
                        .HasColumnType("float")
                        .HasColumnName("Confidence");

                    b1.HasKey("AttackDetectionId");

                    b1.ToTable("AttackDetections");

                    b1.WithOwner()
                        .HasForeignKey("AttackDetectionId");
                });

                b.OwnsOne("NetworkAttackDetectionPlatform.Domain.ValueObjects.IpAddress", "DestinationIp", b1 =>
                {
                    b1.Property<Guid>("AttackDetectionId")
                        .HasColumnType("uniqueidentifier");

                    b1.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("DestinationIp");

                    b1.HasKey("AttackDetectionId");

                    b1.ToTable("AttackDetections");

                    b1.WithOwner()
                        .HasForeignKey("AttackDetectionId");
                });

                b.OwnsOne("NetworkAttackDetectionPlatform.Domain.ValueObjects.Port", "DestinationPort", b1 =>
                {
                    b1.Property<Guid>("AttackDetectionId")
                        .HasColumnType("uniqueidentifier");

                    b1.Property<int>("Value")
                        .HasColumnType("int")
                        .HasColumnName("DestinationPort");

                    b1.HasKey("AttackDetectionId");

                    b1.ToTable("AttackDetections");

                    b1.WithOwner()
                        .HasForeignKey("AttackDetectionId");
                });

                b.OwnsOne("NetworkAttackDetectionPlatform.Domain.ValueObjects.TimeRange", "Occurrence", b1 =>
                {
                    b1.Property<Guid>("AttackDetectionId")
                        .HasColumnType("uniqueidentifier");

                    b1.Property<DateTime>("End")
                        .HasColumnType("datetime2")
                        .HasColumnName("OccurrenceEnd");

                    b1.Property<DateTime>("Start")
                        .HasColumnType("datetime2")
                        .HasColumnName("OccurrenceStart");

                    b1.HasKey("AttackDetectionId");

                    b1.ToTable("AttackDetections");

                    b1.WithOwner()
                        .HasForeignKey("AttackDetectionId");
                });

                b.OwnsOne("NetworkAttackDetectionPlatform.Domain.ValueObjects.IpAddress", "SourceIp", b1 =>
                {
                    b1.Property<Guid>("AttackDetectionId")
                        .HasColumnType("uniqueidentifier");

                    b1.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("SourceIp");

                    b1.HasKey("AttackDetectionId");

                    b1.ToTable("AttackDetections");

                    b1.WithOwner()
                        .HasForeignKey("AttackDetectionId");
                });

                b.OwnsOne("NetworkAttackDetectionPlatform.Domain.ValueObjects.Port", "SourcePort", b1 =>
                {
                    b1.Property<Guid>("AttackDetectionId")
                        .HasColumnType("uniqueidentifier");

                    b1.Property<int>("Value")
                        .HasColumnType("int")
                        .HasColumnName("SourcePort");

                    b1.HasKey("AttackDetectionId");

                    b1.ToTable("AttackDetections");

                    b1.WithOwner()
                        .HasForeignKey("AttackDetectionId");
                });

                b.Navigation("Confidence")
                    .IsRequired();

                b.Navigation("DestinationIp")
                    .IsRequired();

                b.Navigation("DestinationPort")
                    .IsRequired();

                b.Navigation("Occurrence")
                    .IsRequired();

                b.Navigation("SourceIp")
                    .IsRequired();

                b.Navigation("SourcePort")
                    .IsRequired();
            });

            modelBuilder.Entity("NetworkAttackDetectionPlatform.Domain.Entities.Recommendation", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("AttackDetectionId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("AttackDetectionId");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("CreatedAt");

                b.Property<string>("Text")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("Text");

                b.HasKey("Id");

                b.HasIndex("AttackDetectionId");

                b.ToTable("Recommendations", (string)null);
            });

            modelBuilder.Entity("NetworkAttackDetectionPlatform.Domain.Entities.Recommendation", b =>
            {
                b.HasOne("NetworkAttackDetectionPlatform.Domain.Entities.AttackDetection", null)
                    .WithMany("Recommendations")
                    .HasForeignKey("AttackDetectionId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("NetworkAttackDetectionPlatform.Domain.Entities.AttackDetection", b =>
            {
                b.Navigation("Recommendations");
            });
        }
    }
}
