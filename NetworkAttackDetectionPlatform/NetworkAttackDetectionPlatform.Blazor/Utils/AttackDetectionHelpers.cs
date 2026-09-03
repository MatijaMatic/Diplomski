namespace NetworkAttackDetectionPlatform.Blazor.Utils
{
    /// <summary>
    /// Utility class for attack detection UI helpers - mapping enums to display values and CSS classes
    /// </summary>
    public static class AttackDetectionHelpers
    {
        public static string MapAttackType(int attackType) => attackType switch
        {
            0 => "Unknown",
            1 => "Port Scan",
            2 => "DDoS",
            3 => "Malware",
            4 => "Brute Force",
            5 => "Phishing",
            6 => "Data Exfiltration",
            _ => "Unknown"
        };

        public static string MapSeverity(int severity) => severity switch
        {
            0 => "Unknown",
            1 => "Low",
            2 => "Medium",
            3 => "High",
            4 => "Critical",
            _ => "Unknown"
        };

        public static string MapStatus(int status) => status switch
        {
            0 => "New",
            1 => "Analyzed",
            2 => "Confirmed",
            3 => "False Positive",
            4 => "Resolved",
            _ => "Unknown"
        };

        public static string MapProtocol(int protocol) => protocol switch
        {
            0 => "Unknown",
            1 => "TCP",
            2 => "UDP",
            3 => "ICMP",
            4 => "Other",
            _ => "Unknown"
        };

        public static string GetSeverityBadgeClass(int severity) => severity switch
        {
            4 => "bg-danger",
            3 => "bg-warning text-dark",
            2 => "bg-info text-dark",
            1 => "bg-success",
            _ => "bg-secondary"
        };

        public static string GetStatusBadgeClass(int status) => status switch
        {
            0 => "bg-primary",
            1 => "bg-info",
            2 => "bg-danger",
            3 => "bg-warning text-dark",
            4 => "bg-success",
            _ => "bg-secondary"
        };

        public static string GetSeverityIcon(int severity) => severity switch
        {
            4 => "bi-exclamation-triangle-fill",
            3 => "bi-exclamation-circle-fill",
            2 => "bi-info-circle-fill",
            1 => "bi-check-circle-fill",
            _ => "bi-question-circle-fill"
        };

        public static string FormatConfidence(double confidence) => $"{confidence:F1}%";

        public static string GetConfidenceDescription(double confidence) => confidence switch
        {
            >= 90 => "Very high confidence - Immediate action recommended",
            >= 75 => "High confidence - Investigation recommended",
            >= 50 => "Moderate confidence - Monitor closely",
            >= 25 => "Low confidence - May require additional analysis",
            _ => "Very low confidence - Manual verification needed"
        };
    }
}
