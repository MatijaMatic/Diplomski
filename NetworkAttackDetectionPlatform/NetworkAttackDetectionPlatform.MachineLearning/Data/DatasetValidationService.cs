using NetworkAttackDetectionPlatform.MachineLearning.Models;

namespace NetworkAttackDetectionPlatform.MachineLearning.Data;

/// <summary>
/// Validates CICIDS2017 dataset quality and completeness before training.
/// </summary>
public class DatasetValidationService
{
    private static readonly HashSet<string> SupportedLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        "BENIGN",
        "DoS Hulk",
        "DoS GoldenEye",
        "DoS slowloris",
        "DoS Slowhttptest",
        "DDoS",
        "PortScan",
        "Bot",
        "FTP-Patator",
        "SSH-Patator",
        "Web Attack - Brute Force",
        "Web Attack - XSS",
        "Web Attack - Sql Injection",
        "Infiltration",
        "Heartbleed"
    };

    /// <summary>
    /// Validates a collection of training data and returns a detailed report.
    /// </summary>
    /// <param name="data">The training data to validate.</param>
    /// <returns>A validation report with quality metrics and diagnostics.</returns>
    public ValidationReport Validate(IEnumerable<Cicids2017TrainingData> data)
    {
        var report = new ValidationReport();
        var dataList = data.ToList();

        report.TotalRows = dataList.Count;

        if (report.TotalRows == 0)
        {
            report.IsValid = false;
            report.Errors.Add("Dataset is empty - no rows to validate.");
            return report;
        }

        int validRows = 0;
        int invalidRows = 0;
        int missingValues = 0;
        int unknownLabels = 0;

        foreach (var row in dataList)
        {
            bool isRowValid = true;

            // Validate label
            if (string.IsNullOrWhiteSpace(row.Label))
            {
                report.Errors.Add($"Row has empty or null label.");
                isRowValid = false;
            }
            else if (!SupportedLabels.Contains(row.Label))
            {
                report.Warnings.Add($"Unknown label encountered: '{row.Label}'");
                unknownLabels++;
            }

            // Check for invalid numeric values
            var numericValidation = ValidateNumericFeatures(row);
            if (numericValidation.hasInvalidValues)
            {
                report.Warnings.Add($"Row contains {numericValidation.invalidCount} invalid numeric values (NaN, Infinity).");
                missingValues += numericValidation.invalidCount;
            }

            // Check for suspicious patterns
            if (IsAllZeros(row))
            {
                report.Warnings.Add("Row contains all zero values - may indicate corrupted data.");
                isRowValid = false;
            }

            if (isRowValid)
            {
                validRows++;
            }
            else
            {
                invalidRows++;
            }
        }

        report.ValidRows = validRows;
        report.InvalidRows = invalidRows;
        report.MissingValueCount = missingValues;
        report.UnknownLabelCount = unknownLabels;

        // Determine overall validity
        if (report.ValidRows == 0)
        {
            report.IsValid = false;
            report.Errors.Add("No valid rows found in dataset.");
        }
        else if (report.InvalidRows > report.ValidRows * 0.1) // More than 10% invalid
        {
            report.IsValid = false;
            report.Errors.Add($"Too many invalid rows: {report.InvalidRows} ({(double)report.InvalidRows / report.TotalRows * 100:F2}%)");
        }
        else if (unknownLabels > report.TotalRows * 0.05) // More than 5% unknown labels
        {
            report.IsValid = false;
            report.Errors.Add($"Too many unknown labels: {unknownLabels} ({(double)unknownLabels / report.TotalRows * 100:F2}%)");
        }
        else
        {
            report.IsValid = true;
        }

        // Add summary warnings
        if (missingValues > 0)
        {
            report.Warnings.Add($"Dataset contains {missingValues} missing or invalid numeric values.");
        }

        return report;
    }

    /// <summary>
    /// Validates numeric features for NaN, Infinity, and other invalid values.
    /// </summary>
    private (bool hasInvalidValues, int invalidCount) ValidateNumericFeatures(Cicids2017TrainingData row)
    {
        int invalidCount = 0;

        var numericValues = new[]
        {
            row.DestinationPort,
            row.FlowDuration,
            row.TotalFwdPackets,
            row.TotalBackwardPackets,
            row.TotalLengthOfFwdPackets,
            row.TotalLengthOfBwdPackets,
            row.FwdPacketLengthMax,
            row.FwdPacketLengthMin,
            row.FwdPacketLengthMean,
            row.FwdPacketLengthStd,
            row.BwdPacketLengthMax,
            row.BwdPacketLengthMin,
            row.BwdPacketLengthMean,
            row.BwdPacketLengthStd,
            row.FlowBytesPerSecond,
            row.FlowPacketsPerSecond,
            row.FlowIATMean,
            row.FlowIATStd,
            row.FlowIATMax,
            row.FlowIATMin,
            row.FwdIATTotal,
            row.FwdIATMean,
            row.FwdIATStd,
            row.FwdIATMax,
            row.FwdIATMin,
            row.BwdIATTotal,
            row.BwdIATMean,
            row.BwdIATStd,
            row.BwdIATMax,
            row.BwdIATMin,
            row.FwdPSHFlags,
            row.BwdPSHFlags,
            row.FwdURGFlags,
            row.BwdURGFlags,
            row.FwdHeaderLength,
            row.BwdHeaderLength,
            row.FwdPacketsPerSecond,
            row.BwdPacketsPerSecond,
            row.MinPacketLength,
            row.MaxPacketLength,
            row.PacketLengthMean,
            row.PacketLengthStd,
            row.PacketLengthVariance,
            row.FINFlagCount,
            row.SYNFlagCount,
            row.RSTFlagCount,
            row.PSHFlagCount,
            row.ACKFlagCount,
            row.URGFlagCount,
            row.CWEFlagCount,
            row.ECEFlagCount,
            row.DownUpRatio,
            row.AveragePacketSize,
            row.AvgFwdSegmentSize,
            row.AvgBwdSegmentSize,
            row.FwdHeaderLengthDuplicate,
            row.FwdAvgBytesPerBulk,
            row.FwdAvgPacketsPerBulk,
            row.FwdAvgBulkRate,
            row.BwdAvgBytesPerBulk,
            row.BwdAvgPacketsPerBulk,
            row.BwdAvgBulkRate,
            row.SubflowFwdPackets,
            row.SubflowFwdBytes,
            row.SubflowBwdPackets,
            row.SubflowBwdBytes,
            row.InitWinBytesForward,
            row.InitWinBytesBackward,
            row.ActDataPktFwd,
            row.MinSegSizeForward,
            row.ActiveMean,
            row.ActiveStd,
            row.ActiveMax,
            row.ActiveMin,
            row.IdleMean,
            row.IdleStd,
            row.IdleMax,
            row.IdleMin
        };

        foreach (var value in numericValues)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                invalidCount++;
            }
        }

        return (invalidCount > 0, invalidCount);
    }

    /// <summary>
    /// Checks if all numeric features are zero (potential corrupted row indicator).
    /// </summary>
    private bool IsAllZeros(Cicids2017TrainingData row)
    {
        return row.FlowDuration == 0 &&
               row.TotalFwdPackets == 0 &&
               row.TotalBackwardPackets == 0 &&
               row.TotalLengthOfFwdPackets == 0 &&
               row.TotalLengthOfBwdPackets == 0 &&
               row.FlowBytesPerSecond == 0 &&
               row.FlowPacketsPerSecond == 0;
    }

    /// <summary>
    /// Gets the list of supported attack labels for the CICIDS2017 dataset.
    /// </summary>
    public IReadOnlySet<string> GetSupportedLabels() => SupportedLabels;
}
