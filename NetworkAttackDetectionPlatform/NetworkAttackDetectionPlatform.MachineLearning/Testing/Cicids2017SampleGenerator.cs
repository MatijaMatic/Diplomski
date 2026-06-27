using NetworkAttackDetectionPlatform.MachineLearning.Models;

namespace NetworkAttackDetectionPlatform.MachineLearning.Testing;

/// <summary>
/// Generates sample CICIDS2017 data for testing when real dataset is unavailable.
/// </summary>
public class Cicids2017SampleGenerator
{
    private readonly Random _random;

    public Cicids2017SampleGenerator(int seed = 42)
    {
        _random = new Random(seed);
    }

    /// <summary>
    /// Generates sample CICIDS2017 training data.
    /// </summary>
    /// <param name="count">Number of samples to generate.</param>
    /// <param name="includeAllLabels">Whether to include all attack types or just a subset.</param>
    /// <returns>Collection of sample training data.</returns>
    public IEnumerable<Cicids2017TrainingData> GenerateSamples(int count, bool includeAllLabels = true)
    {
        var labels = includeAllLabels
            ? new[] { "BENIGN", "DDoS", "PortScan", "Bot", "Web Attack - XSS", "FTP-Patator", "DoS Hulk", "Infiltration" }
            : new[] { "BENIGN", "DDoS", "PortScan" };

        var samples = new List<Cicids2017TrainingData>();

        for (int i = 0; i < count; i++)
        {
            var label = labels[_random.Next(labels.Length)];
            samples.Add(GenerateSingleSample(label));
        }

        return samples;
    }

    /// <summary>
    /// Generates a single sample with characteristics typical for the given attack type.
    /// </summary>
    private Cicids2017TrainingData GenerateSingleSample(string label)
    {
        var sample = new Cicids2017TrainingData
        {
            Label = label
        };

        // Generate features based on attack type
        switch (label)
        {
            case "BENIGN":
                GenerateBenignFeatures(sample);
                break;
            case "DDoS":
            case "DoS Hulk":
            case "DoS GoldenEye":
                GenerateDDoSFeatures(sample);
                break;
            case "PortScan":
                GeneratePortScanFeatures(sample);
                break;
            case "Bot":
                GenerateBotFeatures(sample);
                break;
            case "FTP-Patator":
            case "SSH-Patator":
                GenerateBruteForceFeatures(sample);
                break;
            case "Web Attack - XSS":
            case "Web Attack - Sql Injection":
            case "Web Attack - Brute Force":
                GenerateWebAttackFeatures(sample);
                break;
            default:
                GenerateBenignFeatures(sample);
                break;
        }

        return sample;
    }

    private void GenerateBenignFeatures(Cicids2017TrainingData sample)
    {
        sample.DestinationPort = _random.Next(80, 443);
        sample.FlowDuration = _random.Next(1000, 100000);
        sample.TotalFwdPackets = _random.Next(5, 50);
        sample.TotalBackwardPackets = _random.Next(5, 50);
        sample.TotalLengthOfFwdPackets = _random.Next(500, 5000);
        sample.TotalLengthOfBwdPackets = _random.Next(500, 5000);
        sample.FlowBytesPerSecond = _random.Next(1000, 10000);
        sample.FlowPacketsPerSecond = _random.Next(10, 100);
        sample.Protocol = 6; // TCP

        SetCommonFeatures(sample);
    }

    private void GenerateDDoSFeatures(Cicids2017TrainingData sample)
    {
        sample.DestinationPort = 80;
        sample.FlowDuration = _random.Next(100, 5000); // Short duration
        sample.TotalFwdPackets = _random.Next(100, 1000); // High packet count
        sample.TotalBackwardPackets = _random.Next(0, 10); // Low response
        sample.TotalLengthOfFwdPackets = _random.Next(10000, 100000);
        sample.TotalLengthOfBwdPackets = _random.Next(0, 1000);
        sample.FlowBytesPerSecond = _random.Next(50000, 500000); // High bandwidth
        sample.FlowPacketsPerSecond = _random.Next(500, 5000); // High packet rate
        sample.SYNFlagCount = _random.Next(50, 200);
        sample.Protocol = 6;

        SetCommonFeatures(sample);
    }

    private void GeneratePortScanFeatures(Cicids2017TrainingData sample)
    {
        sample.DestinationPort = _random.Next(1, 65535); // Random ports
        sample.FlowDuration = _random.Next(10, 100); // Very short
        sample.TotalFwdPackets = _random.Next(1, 5); // Few packets
        sample.TotalBackwardPackets = _random.Next(0, 2); // Minimal response
        sample.TotalLengthOfFwdPackets = _random.Next(40, 200);
        sample.TotalLengthOfBwdPackets = _random.Next(0, 100);
        sample.FlowBytesPerSecond = _random.Next(100, 1000);
        sample.FlowPacketsPerSecond = _random.Next(50, 200);
        sample.SYNFlagCount = 1;
        sample.RSTFlagCount = _random.Next(0, 1);
        sample.Protocol = 6;

        SetCommonFeatures(sample);
    }

    private void GenerateBotFeatures(Cicids2017TrainingData sample)
    {
        sample.DestinationPort = _random.Next(1024, 65535); // High ports
        sample.FlowDuration = _random.Next(10000, 500000); // Long duration
        sample.TotalFwdPackets = _random.Next(10, 100);
        sample.TotalBackwardPackets = _random.Next(10, 100);
        sample.TotalLengthOfFwdPackets = _random.Next(1000, 10000);
        sample.TotalLengthOfBwdPackets = _random.Next(1000, 10000);
        sample.FlowBytesPerSecond = _random.Next(500, 5000);
        sample.FlowPacketsPerSecond = _random.Next(5, 50);
        sample.Protocol = 6;

        SetCommonFeatures(sample);
    }

    private void GenerateBruteForceFeatures(Cicids2017TrainingData sample)
    {
        sample.DestinationPort = _random.Next(21, 22) == 21 ? 21 : 22; // FTP or SSH
        sample.FlowDuration = _random.Next(5000, 50000);
        sample.TotalFwdPackets = _random.Next(20, 100); // Multiple attempts
        sample.TotalBackwardPackets = _random.Next(20, 100);
        sample.TotalLengthOfFwdPackets = _random.Next(2000, 20000);
        sample.TotalLengthOfBwdPackets = _random.Next(2000, 20000);
        sample.FlowBytesPerSecond = _random.Next(2000, 20000);
        sample.FlowPacketsPerSecond = _random.Next(20, 200);
        sample.PSHFlagCount = _random.Next(10, 50);
        sample.ACKFlagCount = _random.Next(10, 50);
        sample.Protocol = 6;

        SetCommonFeatures(sample);
    }

    private void GenerateWebAttackFeatures(Cicids2017TrainingData sample)
    {
        sample.DestinationPort = 80;
        sample.FlowDuration = _random.Next(1000, 50000);
        sample.TotalFwdPackets = _random.Next(10, 100);
        sample.TotalBackwardPackets = _random.Next(10, 100);
        sample.TotalLengthOfFwdPackets = _random.Next(5000, 50000); // Large payloads
        sample.TotalLengthOfBwdPackets = _random.Next(5000, 50000);
        sample.FlowBytesPerSecond = _random.Next(5000, 50000);
        sample.FlowPacketsPerSecond = _random.Next(50, 500);
        sample.PSHFlagCount = _random.Next(5, 20);
        sample.ACKFlagCount = _random.Next(5, 20);
        sample.Protocol = 6;

        SetCommonFeatures(sample);
    }

    private void SetCommonFeatures(Cicids2017TrainingData sample)
    {
        // Packet length statistics
        sample.FwdPacketLengthMax = _random.Next(100, 1500);
        sample.FwdPacketLengthMin = _random.Next(40, 100);
        sample.FwdPacketLengthMean = (sample.FwdPacketLengthMax + sample.FwdPacketLengthMin) / 2.0f;
        sample.FwdPacketLengthStd = _random.Next(10, 100);

        sample.BwdPacketLengthMax = _random.Next(100, 1500);
        sample.BwdPacketLengthMin = _random.Next(40, 100);
        sample.BwdPacketLengthMean = (sample.BwdPacketLengthMax + sample.BwdPacketLengthMin) / 2.0f;
        sample.BwdPacketLengthStd = _random.Next(10, 100);

        // IAT statistics
        sample.FlowIATMean = _random.Next(100, 10000);
        sample.FlowIATStd = _random.Next(100, 5000);
        sample.FlowIATMax = _random.Next(1000, 50000);
        sample.FlowIATMin = _random.Next(1, 100);

        sample.FwdIATTotal = _random.Next(1000, 100000);
        sample.FwdIATMean = _random.Next(100, 10000);
        sample.FwdIATStd = _random.Next(100, 5000);
        sample.FwdIATMax = _random.Next(1000, 50000);
        sample.FwdIATMin = _random.Next(1, 100);

        sample.BwdIATTotal = _random.Next(1000, 100000);
        sample.BwdIATMean = _random.Next(100, 10000);
        sample.BwdIATStd = _random.Next(100, 5000);
        sample.BwdIATMax = _random.Next(1000, 50000);
        sample.BwdIATMin = _random.Next(1, 100);

        // Header lengths
        sample.FwdHeaderLength = _random.Next(20, 60);
        sample.BwdHeaderLength = _random.Next(20, 60);

        // Packet statistics
        sample.MinPacketLength = _random.Next(40, 100);
        sample.MaxPacketLength = _random.Next(1000, 1500);
        sample.PacketLengthMean = (sample.MinPacketLength + sample.MaxPacketLength) / 2.0f;
        sample.PacketLengthStd = _random.Next(50, 200);
        sample.PacketLengthVariance = sample.PacketLengthStd * sample.PacketLengthStd;

        sample.AveragePacketSize = _random.Next(100, 1000);
        sample.AvgFwdSegmentSize = _random.Next(100, 1000);
        sample.AvgBwdSegmentSize = _random.Next(100, 1000);

        // Packets per second
        sample.FwdPacketsPerSecond = _random.Next(1, 100);
        sample.BwdPacketsPerSecond = _random.Next(1, 100);

        // Bulk features
        sample.FwdAvgBytesPerBulk = _random.Next(0, 10000);
        sample.FwdAvgPacketsPerBulk = _random.Next(0, 100);
        sample.FwdAvgBulkRate = _random.Next(0, 10000);
        sample.BwdAvgBytesPerBulk = _random.Next(0, 10000);
        sample.BwdAvgPacketsPerBulk = _random.Next(0, 100);
        sample.BwdAvgBulkRate = _random.Next(0, 10000);

        // Subflow features
        sample.SubflowFwdPackets = _random.Next(1, 50);
        sample.SubflowFwdBytes = _random.Next(100, 5000);
        sample.SubflowBwdPackets = _random.Next(1, 50);
        sample.SubflowBwdBytes = _random.Next(100, 5000);

        // Window bytes
        sample.InitWinBytesForward = _random.Next(8192, 65535);
        sample.InitWinBytesBackward = _random.Next(8192, 65535);

        // Active/Idle statistics
        sample.ActiveMean = _random.Next(1000, 100000);
        sample.ActiveStd = _random.Next(100, 10000);
        sample.ActiveMax = _random.Next(10000, 500000);
        sample.ActiveMin = _random.Next(1, 1000);

        sample.IdleMean = _random.Next(1000, 100000);
        sample.IdleStd = _random.Next(100, 10000);
        sample.IdleMax = _random.Next(10000, 500000);
        sample.IdleMin = _random.Next(1, 1000);

        // Other features
        sample.ActDataPktFwd = _random.Next(1, 50);
        sample.MinSegSizeForward = _random.Next(20, 100);
        sample.DownUpRatio = _random.Next(0, 10);

        // Initialize flags if not set
        if (sample.FINFlagCount == 0) sample.FINFlagCount = _random.Next(0, 2);
        if (sample.SYNFlagCount == 0) sample.SYNFlagCount = _random.Next(0, 2);
        if (sample.RSTFlagCount == 0) sample.RSTFlagCount = _random.Next(0, 2);
        if (sample.PSHFlagCount == 0) sample.PSHFlagCount = _random.Next(0, 5);
        if (sample.ACKFlagCount == 0) sample.ACKFlagCount = _random.Next(0, 10);
        if (sample.URGFlagCount == 0) sample.URGFlagCount = _random.Next(0, 1);
        if (sample.CWEFlagCount == 0) sample.CWEFlagCount = _random.Next(0, 1);
        if (sample.ECEFlagCount == 0) sample.ECEFlagCount = _random.Next(0, 1);

        sample.FwdPSHFlags = _random.Next(0, 2);
        sample.BwdPSHFlags = _random.Next(0, 2);
        sample.FwdURGFlags = _random.Next(0, 1);
        sample.BwdURGFlags = _random.Next(0, 1);
    }

    /// <summary>
    /// Writes generated samples to a CSV file.
    /// </summary>
    public async Task WriteSampleCsvAsync(string filePath, int sampleCount, bool includeAllLabels = true)
    {
        var samples = GenerateSamples(sampleCount, includeAllLabels).ToList();

        using var writer = new StreamWriter(filePath);

        // Write header
        await writer.WriteLineAsync(
            "DestinationPort,FlowDuration,TotalFwdPackets,TotalBackwardPackets," +
            "TotalLengthOfFwdPackets,TotalLengthOfBwdPackets,FwdPacketLengthMax,FwdPacketLengthMin," +
            "FwdPacketLengthMean,FwdPacketLengthStd,BwdPacketLengthMax,BwdPacketLengthMin," +
            "BwdPacketLengthMean,BwdPacketLengthStd,FlowBytesPerSecond,FlowPacketsPerSecond," +
            "FlowIATMean,FlowIATStd,FlowIATMax,FlowIATMin,FwdIATTotal,FwdIATMean,FwdIATStd," +
            "FwdIATMax,FwdIATMin,BwdIATTotal,BwdIATMean,BwdIATStd,BwdIATMax,BwdIATMin," +
            "FwdPSHFlags,BwdPSHFlags,FwdURGFlags,BwdURGFlags,FINFlagCount,SYNFlagCount," +
            "RSTFlagCount,PSHFlagCount,ACKFlagCount,URGFlagCount,CWEFlagCount,ECEFlagCount," +
            "FwdHeaderLength,BwdHeaderLength,FwdPacketsPerSecond,BwdPacketsPerSecond," +
            "MinPacketLength,MaxPacketLength,PacketLengthMean,PacketLengthStd,PacketLengthVariance," +
            "DownUpRatio,AveragePacketSize,AvgFwdSegmentSize,AvgBwdSegmentSize," +
            "FwdAvgBytesPerBulk,FwdAvgPacketsPerBulk,FwdAvgBulkRate,BwdAvgBytesPerBulk," +
            "BwdAvgPacketsPerBulk,BwdAvgBulkRate,SubflowFwdPackets,SubflowFwdBytes," +
            "SubflowBwdPackets,SubflowBwdBytes,InitWinBytesForward,InitWinBytesBackward," +
            "ActiveMean,ActiveStd,ActiveMax,ActiveMin,IdleMean,IdleStd,IdleMax,IdleMin," +
            "ActDataPktFwd,MinSegSizeForward,Protocol,Label");

        // Write data rows
        foreach (var sample in samples)
        {
            await writer.WriteLineAsync(
                $"{sample.DestinationPort},{sample.FlowDuration},{sample.TotalFwdPackets},{sample.TotalBackwardPackets}," +
                $"{sample.TotalLengthOfFwdPackets},{sample.TotalLengthOfBwdPackets},{sample.FwdPacketLengthMax},{sample.FwdPacketLengthMin}," +
                $"{sample.FwdPacketLengthMean},{sample.FwdPacketLengthStd},{sample.BwdPacketLengthMax},{sample.BwdPacketLengthMin}," +
                $"{sample.BwdPacketLengthMean},{sample.BwdPacketLengthStd},{sample.FlowBytesPerSecond},{sample.FlowPacketsPerSecond}," +
                $"{sample.FlowIATMean},{sample.FlowIATStd},{sample.FlowIATMax},{sample.FlowIATMin},{sample.FwdIATTotal},{sample.FwdIATMean},{sample.FwdIATStd}," +
                $"{sample.FwdIATMax},{sample.FwdIATMin},{sample.BwdIATTotal},{sample.BwdIATMean},{sample.BwdIATStd},{sample.BwdIATMax},{sample.BwdIATMin}," +
                $"{sample.FwdPSHFlags},{sample.BwdPSHFlags},{sample.FwdURGFlags},{sample.BwdURGFlags},{sample.FINFlagCount},{sample.SYNFlagCount}," +
                $"{sample.RSTFlagCount},{sample.PSHFlagCount},{sample.ACKFlagCount},{sample.URGFlagCount},{sample.CWEFlagCount},{sample.ECEFlagCount}," +
                $"{sample.FwdHeaderLength},{sample.BwdHeaderLength},{sample.FwdPacketsPerSecond},{sample.BwdPacketsPerSecond}," +
                $"{sample.MinPacketLength},{sample.MaxPacketLength},{sample.PacketLengthMean},{sample.PacketLengthStd},{sample.PacketLengthVariance}," +
                $"{sample.DownUpRatio},{sample.AveragePacketSize},{sample.AvgFwdSegmentSize},{sample.AvgBwdSegmentSize}," +
                $"{sample.FwdAvgBytesPerBulk},{sample.FwdAvgPacketsPerBulk},{sample.FwdAvgBulkRate},{sample.BwdAvgBytesPerBulk}," +
                $"{sample.BwdAvgPacketsPerBulk},{sample.BwdAvgBulkRate},{sample.SubflowFwdPackets},{sample.SubflowFwdBytes}," +
                $"{sample.SubflowBwdPackets},{sample.SubflowBwdBytes},{sample.InitWinBytesForward},{sample.InitWinBytesBackward}," +
                $"{sample.ActiveMean},{sample.ActiveStd},{sample.ActiveMax},{sample.ActiveMin},{sample.IdleMean},{sample.IdleStd},{sample.IdleMax},{sample.IdleMin}," +
                $"{sample.ActDataPktFwd},{sample.MinSegSizeForward},{sample.Protocol},{sample.Label}");
        }

        Console.WriteLine($"Generated {sampleCount} sample rows and saved to: {filePath}");
    }
}
