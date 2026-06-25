namespace NetworkAttackDetectionPlatform.Domain.Enums
{
    public enum AttackTypeEnum
    {
        Unknown = 0,
        PortScan = 1,
        DDoS = 2,
        Malware = 3,
        BruteForce = 4,
        Phishing = 5,
        DataExfiltration = 6
    }
}