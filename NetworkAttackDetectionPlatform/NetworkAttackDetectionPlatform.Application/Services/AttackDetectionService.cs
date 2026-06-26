using System;
using System.Collections.Generic;
using System.Linq;
using NetworkAttackDetectionPlatform.Application.DTOs;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using NetworkAttackDetectionPlatform.Domain.Entities;
using NetworkAttackDetectionPlatform.Domain.Enums;
using NetworkAttackDetectionPlatform.Domain.Interfaces;
using NetworkAttackDetectionPlatform.Domain.ValueObjects;

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

        public PagedResult<AttackDetectionDto> GetDetections(int pageNumber, int pageSize, int? attackType = null, int? severity = null, DateTime? startDate = null, DateTime? endDate = null, double? minConfidence = null, double? maxConfidence = null)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 50;

            var (items, total) = _repo.GetDetections(pageNumber, pageSize, attackType, severity, null, startDate, endDate, minConfidence, maxConfidence);

            var dtoItems = items.Select(MapToDto).ToList();

            return new PagedResult<AttackDetectionDto>
            {
                Items = dtoItems,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public void SetStatus(Guid id, string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName)) throw new ArgumentException("Status must be provided.", nameof(statusName));

            var existing = _repo.GetById(id) ?? throw new InvalidOperationException("AttackDetection not found.");

            if (!Enum.TryParse<DetectionStatusEnum>(statusName, ignoreCase: true, out var status))
                throw new ArgumentException("Invalid status value.", nameof(statusName));

            switch (status)
            {
                case DetectionStatusEnum.Analyzed:
                    existing.MarkAnalyzed();
                    break;
                case DetectionStatusEnum.Confirmed:
                    existing.Confirm();
                    break;
                case DetectionStatusEnum.FalsePositive:
                    existing.MarkFalsePositive();
                    break;
                case DetectionStatusEnum.Resolved:
                    existing.Resolve();
                    break;
                case DetectionStatusEnum.New:
                    throw new InvalidOperationException("Setting status to New is not supported.");
                default:
                    throw new ArgumentException("Unsupported status value.", nameof(statusName));
            }

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
                Status = (int)src.Status,
                CreatedAt = src.CreatedAt,
                UpdatedAt = src.UpdatedAt,
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
