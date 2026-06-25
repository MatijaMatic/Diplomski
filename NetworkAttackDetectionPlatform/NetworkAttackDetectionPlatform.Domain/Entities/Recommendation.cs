using System;
using NetworkAttackDetectionPlatform.Domain.Constants;

namespace NetworkAttackDetectionPlatform.Domain.Entities
{
    public sealed class Recommendation
    {
        public Guid Id { get; private set; }

        public Guid AttackDetectionId { get; private set; }

        public string Text { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private Recommendation() { }

        private Recommendation(Guid attackDetectionId, string text)
        {
            Id = Guid.NewGuid();
            AttackDetectionId = attackDetectionId;
            SetText(text);
            CreatedAt = DateTime.UtcNow;
        }

        public static Recommendation Create(Guid attackDetectionId, string text)
        {
            if (attackDetectionId == Guid.Empty)
                throw new ArgumentException("AttackDetectionId must be provided.", nameof(attackDetectionId));

            return new Recommendation(attackDetectionId, text);
        }

        private void SetText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Recommendation text cannot be empty.", nameof(text));

            if (text.Length > DomainConstants.MaxRecommendationLength)
                throw new ArgumentException($"Recommendation text cannot exceed {DomainConstants.MaxRecommendationLength} characters.", nameof(text));

            Text = text;
        }
    }
}