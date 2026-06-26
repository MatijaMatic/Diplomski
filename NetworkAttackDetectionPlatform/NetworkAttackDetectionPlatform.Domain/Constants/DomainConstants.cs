namespace NetworkAttackDetectionPlatform.Domain.Constants
{
    internal static class DomainConstants
    {
        public const int MinPort = 1;
        public const int MaxPort = 65535;

        public const double MinConfidence = 0.0;
        public const double MaxConfidence = 100.0;

        public const int MaxRecommendationLength = 2000;
        public const int MaxIpLength = 45; // accommodates IPv6
    }
}