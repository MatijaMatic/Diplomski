using System;
using System.Collections.Generic;
using System.Linq;
using NetworkAttackDetectionPlatform.Application.DTOs;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using NetworkAttackDetectionPlatform.Domain.Entities;
using NetworkAttackDetectionPlatform.Domain.Interfaces;
using NetworkAttackDetectionPlatform.Domain.ValueObjects;
using NetworkAttackDetectionPlatform.Domain.Enums;

namespace NetworkAttackDetectionPlatform.Application.Services
{
    public class AttackDetectionService : IAttackDetectionService
    {
        private readonly IAttackDetectionRepository _repo;

        public AttackDetectionService(IAttackDetectionRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public IEnumerable<AttackDetectionDto> GetAll()
        {
            var items = _repo.GetAll();
            return items.Select(MapToDto).ToList();
        }

        public AttackDetectionDto? GetById(Guid id)
        {
            var item = _repo.GetById(id);
            return item == null ? null : MapToDto(item);
        }

        public AttackDetectionDto Create(CreateAttackDetectionDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var attack = AttackDetection.Create(
                new IpAddress(dto.SourceIp),
                new IpAddress(dto.DestinationIp),
                new Port(dto.SourcePort),
                new Port(dto.DestinationPort),
                (ProtocolEnum)dto.Protocol,
                (AttackTypeEnum)dto.AttackType,
                (SeverityLevelEnum)dto.Severity,
                new ConfidenceScore(dto.Confidence),
                new TimeRange(dto.OccurrenceStart, dto.OccurrenceEnd)
            );

            _repo.Add(attack);
            return MapToDto(attack);
        }

        public void AddRecommendation(Guid attackDetectionId, string text)
        {
            var attack = _repo.GetById(attackDetectionId) ?? throw new InvalidOperationException("AttackDetection not found.");
            attack.AddRecommendation(text);
            _repo.Update(attack);
        }

        public void Remove(Guid id)
        {
            _repo.Remove(id);
        }

        public void Update(AttackDetectionDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existing = _repo.GetById(dto.Id) ?? throw new InvalidOperationException("AttackDetection not found.");

            // Replace only mutable parts: Confidence and Recommendations are managed via domain methods
            existing.ReplaceConfidence(new ConfidenceScore(dto.Confidence));

            // For simplicity, we won't sync recommendations here. Expect clients to use recommendation service.

            _repo.Update(existing);
        }

        private static AttackDetectionDto MapToDto(AttackDetection src)
        {
            return new AttackDetectionDto
            {
                Id = src.Id,
                SourceIp = src.SourceIp.Value,
                DestinationIp = src.DestinationIp.Value,
                SourcePort = src.SourcePort.Value,
                DestinationPort = src.DestinationPort.Value,
                Protocol = (int)src.Protocol,
                AttackType = (int)src.AttackType,
                Severity = (int)src.Severity,
                Confidence = src.Confidence.Value,
                OccurrenceStart = src.Occurrence.Start,
                OccurrenceEnd = src.Occurrence.End,
                Recommendations = src.Recommendations.Select(r => new RecommendationDto
                {
                    Id = r.Id,
                    AttackDetectionId = r.AttackDetectionId,
                    Text = r.Text,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };
        }
    }
}
