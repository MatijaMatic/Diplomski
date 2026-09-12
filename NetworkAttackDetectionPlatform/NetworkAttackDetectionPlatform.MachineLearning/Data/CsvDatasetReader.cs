using System.Globalization;
using NetworkAttackDetectionPlatform.MachineLearning.Models;

namespace NetworkAttackDetectionPlatform.MachineLearning.Data;

/// <summary>
/// Reads CICIDS2017 CSV files and converts rows into training data objects.
/// </summary>
public class CsvDatasetReader
{
    private const char Delimiter = ',';

    /// <summary>
    /// Reads a CICIDS2017 CSV file and returns training data rows.
    /// </summary>
    /// <param name="filePath">Path to the CSV file.</param>
    /// <param name="skipHeader">Whether to skip the first row as header.</param>
    /// <returns>Collection of training data objects.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    public async Task<IEnumerable<Cicids2017TrainingData>> ReadAsync(string filePath, bool skipHeader = true)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Dataset file not found: {filePath}");
        }

        var trainingData = new List<Cicids2017TrainingData>();
        var lines = await File.ReadAllLinesAsync(filePath);

        var startIndex = skipHeader ? 1 : 0;

        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            try
            {
                var row = ParseRow(line, i + 1);
                if (row != null)
                {
                    trainingData.Add(row);
                }
            }
            catch (Exception ex)
            {
                // Log malformed row but continue processing
                Console.WriteLine($"Warning: Skipping malformed row {i + 1}: {ex.Message}");
            }
        }

        return trainingData;
    }

    /// <summary>
    /// Parses a single CSV row into a training data object.
    /// </summary>
    private Cicids2017TrainingData? ParseRow(string line, int rowNumber)
    {
        var columns = line.Split(Delimiter);

        if (columns.Length < 79)
        {
            throw new FormatException($"Expected at least 79 columns, found {columns.Length}");
        }

        try
        {
            var data = new Cicids2017TrainingData
            {
                DestinationPort = ParseFloat(columns[0]),
                FlowDuration = ParseFloat(columns[1]),
                TotalFwdPackets = ParseFloat(columns[2]),
                TotalBackwardPackets = ParseFloat(columns[3]),
                TotalLengthOfFwdPackets = ParseFloat(columns[4]),
                TotalLengthOfBwdPackets = ParseFloat(columns[5]),
                FwdPacketLengthMax = ParseFloat(columns[6]),
                FwdPacketLengthMin = ParseFloat(columns[7]),
                FwdPacketLengthMean = ParseFloat(columns[8]),
                FwdPacketLengthStd = ParseFloat(columns[9]),
                BwdPacketLengthMax = ParseFloat(columns[10]),
                BwdPacketLengthMin = ParseFloat(columns[11]),
                BwdPacketLengthMean = ParseFloat(columns[12]),
                BwdPacketLengthStd = ParseFloat(columns[13]),
                FlowBytesPerSecond = ParseFloat(columns[14]),
                FlowPacketsPerSecond = ParseFloat(columns[15]),
                FlowIATMean = ParseFloat(columns[16]),
                FlowIATStd = ParseFloat(columns[17]),
                FlowIATMax = ParseFloat(columns[18]),
                FlowIATMin = ParseFloat(columns[19]),
                FwdIATTotal = ParseFloat(columns[20]),
                FwdIATMean = ParseFloat(columns[21]),
                FwdIATStd = ParseFloat(columns[22]),
                FwdIATMax = ParseFloat(columns[23]),
                FwdIATMin = ParseFloat(columns[24]),
                BwdIATTotal = ParseFloat(columns[25]),
                BwdIATMean = ParseFloat(columns[26]),
                BwdIATStd = ParseFloat(columns[27]),
                BwdIATMax = ParseFloat(columns[28]),
                BwdIATMin = ParseFloat(columns[29]),
                FwdPSHFlags = ParseFloat(columns[30]),
                BwdPSHFlags = ParseFloat(columns[31]),
                FwdURGFlags = ParseFloat(columns[32]),
                BwdURGFlags = ParseFloat(columns[33]),
                FINFlagCount = ParseFloat(columns[34]),
                SYNFlagCount = ParseFloat(columns[35]),
                RSTFlagCount = ParseFloat(columns[36]),
                PSHFlagCount = ParseFloat(columns[37]),
                ACKFlagCount = ParseFloat(columns[38]),
                URGFlagCount = ParseFloat(columns[39]),
                CWEFlagCount = ParseFloat(columns[40]),
                ECEFlagCount = ParseFloat(columns[41]),
                FwdHeaderLength = ParseFloat(columns[42]),
                BwdHeaderLength = ParseFloat(columns[43]),
                FwdPacketsPerSecond = ParseFloat(columns[44]),
                BwdPacketsPerSecond = ParseFloat(columns[45]),
                MinPacketLength = ParseFloat(columns[46]),
                MaxPacketLength = ParseFloat(columns[47]),
                PacketLengthMean = ParseFloat(columns[48]),
                PacketLengthStd = ParseFloat(columns[49]),
                PacketLengthVariance = ParseFloat(columns[50]),
                DownUpRatio = ParseFloat(columns[51]),
                AveragePacketSize = ParseFloat(columns[52]),
                AvgFwdSegmentSize = ParseFloat(columns[53]),
                AvgBwdSegmentSize = ParseFloat(columns[54]),
                FwdAvgBytesPerBulk = ParseFloat(columns[55]),
                FwdAvgPacketsPerBulk = ParseFloat(columns[56]),
                FwdAvgBulkRate = ParseFloat(columns[57]),
                BwdAvgBytesPerBulk = ParseFloat(columns[58]),
                BwdAvgPacketsPerBulk = ParseFloat(columns[59]),
                BwdAvgBulkRate = ParseFloat(columns[60]),
                SubflowFwdPackets = ParseFloat(columns[61]),
                SubflowFwdBytes = ParseFloat(columns[62]),
                SubflowBwdPackets = ParseFloat(columns[63]),
                SubflowBwdBytes = ParseFloat(columns[64]),
                InitWinBytesForward = ParseFloat(columns[65]),
                InitWinBytesBackward = ParseFloat(columns[66]),
                ActiveMean = ParseFloat(columns[67]),
                ActiveStd = ParseFloat(columns[68]),
                ActiveMax = ParseFloat(columns[69]),
                ActiveMin = ParseFloat(columns[70]),
                IdleMean = ParseFloat(columns[71]),
                IdleStd = ParseFloat(columns[72]),
                IdleMax = ParseFloat(columns[73]),
                IdleMin = ParseFloat(columns[74]),
                ActDataPktFwd = ParseFloat(columns[75]),
                MinSegSizeForward = ParseFloat(columns[76]),
                Protocol = ParseFloat(columns[77]),
                Label = columns[78].Trim()
            };

            return data;
        }
        catch (Exception ex)
        {
            throw new FormatException($"Error parsing row {rowNumber}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Parses a string to float, handling various numeric formats and edge cases.
    /// </summary>
    private float ParseFloat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0f;
        }

        value = value.Trim();

        // Handle infinity and NaN
        if (value.Equals("Infinity", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("Inf", StringComparison.OrdinalIgnoreCase))
        {
            return float.MaxValue;
        }

        if (value.Equals("-Infinity", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("-Inf", StringComparison.OrdinalIgnoreCase))
        {
            return float.MinValue;
        }

        if (value.Equals("NaN", StringComparison.OrdinalIgnoreCase))
        {
            return 0f;
        }

        if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            return result;
        }

        return 0f;
    }
}
