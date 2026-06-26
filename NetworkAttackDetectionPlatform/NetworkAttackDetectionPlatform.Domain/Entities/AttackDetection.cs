using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NetworkAttackDetectionPlatform.Domain.Enums;
using NetworkAttackDetectionPlatform.Domain.ValueObjects;

namespace NetworkAttackDetectionPlatform.Domain.Entities
{
    public sealed class AttackDetection
    {
        public Guid Id { get; private set; }

        public IpAddress SourceIp { get; private set; }

        public IpAddress DestinationIp { get; private set; }

        public Port SourcePort { get; private set; }

        public Port DestinationPort { get; private set; }

        public ProtocolEnum Protocol { get; private set; }

        public AttackTypeEnum AttackType { get; private set; }

        public SeverityLevelEnum Severity { get; private set; }

        public ConfidenceScore Confidence { get; private set; }

        public TimeRange Occurrence { get; private set; }

        public DetectionStatusEnum Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private readonly List<Recommendation> _recommendations = new();
        public IReadOnlyCollection<Recommendation> Recommendations => new ReadOnlyCollection<Recommendation>(_recommendations);

        private AttackDetection() { }

        private AttackDetection(
            Guid id,
            IpAddress sourceIp,
            IpAddress destinationIp,
            Port sourcePort,
            Port destinationPort,
            ProtocolEnum protocol,
            AttackTypeEnum attackType,
            SeverityLevelEnum severity,
            ConfidenceScore confidence,
            TimeRange occurrence)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            SourceIp = sourceIp ?? throw new ArgumentNullException(nameof(sourceIp));
            DestinationIp = destinationIp ?? throw new ArgumentNullException(nameof(destinationIp));
            SourcePort = sourcePort ?? throw new ArgumentNullException(nameof(sourcePort));
            DestinationPort = destinationPort ?? throw new ArgumentNullException(nameof(destinationPort));
            Protocol = protocol;
            AttackType = attackType;
            Severity = severity;
            Confidence = confidence ?? throw new ArgumentNullException(nameof(confidence));
            Occurrence = occurrence ?? throw new ArgumentNullException(nameof(occurrence));

            Status = DetectionStatusEnum.New;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public static AttackDetection Create(
            IpAddress sourceIp,
            IpAddress destinationIp,
            Port sourcePort,
            Port destinationPort,
            ProtocolEnum protocol,
            AttackTypeEnum attackType,
            SeverityLevelEnum severity,
            ConfidenceScore confidence,
            TimeRange occurrence)
        {
            return new AttackDetection(
                Guid.NewGuid(),
                sourceIp,
                destinationIp,
                sourcePort,
                destinationPort,
                protocol,
                attackType,
                severity,
                confidence,
                occurrence);
        }

        public void AddRecommendation(string text)
        {
            var recommendation = Recommendation.Create(Id, text);
            _recommendations.Add(recommendation);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveRecommendation(Guid recommendationId)
        {
            var rec = _recommendations.FirstOrDefault(r => r.Id == recommendationId);
            if (rec == null)
                throw new InvalidOperationException("Recommendation not found.");

            _recommendations.Remove(rec);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReplaceConfidence(ConfidenceScore newScore)
        {
            Confidence = newScore ?? throw new ArgumentNullException(nameof(newScore));
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAnalyzed()
        {
            Status = DetectionStatusEnum.Analyzed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Confirm()
        {
            Status = DetectionStatusEnum.Confirmed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFalsePositive()
        {
            Status = DetectionStatusEnum.FalsePositive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Resolve()
        {
            Status = DetectionStatusEnum.Resolved;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
