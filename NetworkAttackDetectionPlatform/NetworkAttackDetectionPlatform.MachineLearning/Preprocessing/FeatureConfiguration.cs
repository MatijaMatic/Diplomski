namespace NetworkAttackDetectionPlatform.MachineLearning.Preprocessing;

/// <summary>
/// Centralized configuration for CICIDS2017 feature selection and ML.NET column names.
/// </summary>
public class FeatureConfiguration
{
    /// <summary>
    /// Name of the label column in the dataset.
    /// </summary>
    public const string LabelColumnName = "Label";

    /// <summary>
    /// Name of the concatenated feature vector column.
    /// </summary>
    public const string FeaturesColumnName = "Features";

    /// <summary>
    /// Name of the predicted label column.
    /// </summary>
    public const string PredictedLabelColumnName = "PredictedLabel";

    /// <summary>
    /// Name of the score column (prediction confidence).
    /// </summary>
    public const string ScoreColumnName = "Score";

    /// <summary>
    /// All numerical features from CICIDS2017 dataset.
    /// These will be concatenated into a single feature vector for training.
    /// </summary>
    public static readonly string[] NumericalFeatures = new[]
    {
        "DestinationPort",
        "FlowDuration",
        "TotalFwdPackets",
        "TotalBackwardPackets",
        "TotalLengthOfFwdPackets",
        "TotalLengthOfBwdPackets",
        "FwdPacketLengthMax",
        "FwdPacketLengthMin",
        "FwdPacketLengthMean",
        "FwdPacketLengthStd",
        "BwdPacketLengthMax",
        "BwdPacketLengthMin",
        "BwdPacketLengthMean",
        "BwdPacketLengthStd",
        "FlowBytesPerSecond",
        "FlowPacketsPerSecond",
        "FlowIATMean",
        "FlowIATStd",
        "FlowIATMax",
        "FlowIATMin",
        "FwdIATTotal",
        "FwdIATMean",
        "FwdIATStd",
        "FwdIATMax",
        "FwdIATMin",
        "BwdIATTotal",
        "BwdIATMean",
        "BwdIATStd",
        "BwdIATMax",
        "BwdIATMin",
        "FwdPSHFlags",
        "BwdPSHFlags",
        "FwdURGFlags",
        "BwdURGFlags",
        "FINFlagCount",
        "SYNFlagCount",
        "RSTFlagCount",
        "PSHFlagCount",
        "ACKFlagCount",
        "URGFlagCount",
        "CWEFlagCount",
        "ECEFlagCount",
        "FwdHeaderLength",
        "BwdHeaderLength",
        "FwdPacketsPerSecond",
        "BwdPacketsPerSecond",
        "MinPacketLength",
        "MaxPacketLength",
        "PacketLengthMean",
        "PacketLengthStd",
        "PacketLengthVariance",
        "DownUpRatio",
        "AveragePacketSize",
        "AvgFwdSegmentSize",
        "AvgBwdSegmentSize",
        "FwdAvgBytesPerBulk",
        "FwdAvgPacketsPerBulk",
        "FwdAvgBulkRate",
        "BwdAvgBytesPerBulk",
        "BwdAvgPacketsPerBulk",
        "BwdAvgBulkRate",
        "SubflowFwdPackets",
        "SubflowFwdBytes",
        "SubflowBwdPackets",
        "SubflowBwdBytes",
        "InitWinBytesForward",
        "InitWinBytesBackward",
        "ActiveMean",
        "ActiveStd",
        "ActiveMax",
        "ActiveMin",
        "IdleMean",
        "IdleStd",
        "IdleMax",
        "IdleMin",
        "ActDataPktFwd",
        "MinSegSizeForward",
        "Protocol"
    };

    /// <summary>
    /// Features that represent counts or categorical values (no negative values expected).
    /// </summary>
    public static readonly string[] CountFeatures = new[]
    {
        "DestinationPort",
        "TotalFwdPackets",
        "TotalBackwardPackets",
        "TotalLengthOfFwdPackets",
        "TotalLengthOfBwdPackets",
        "FwdPSHFlags",
        "BwdPSHFlags",
        "FwdURGFlags",
        "BwdURGFlags",
        "FINFlagCount",
        "SYNFlagCount",
        "RSTFlagCount",
        "PSHFlagCount",
        "ACKFlagCount",
        "URGFlagCount",
        "CWEFlagCount",
        "ECEFlagCount",
        "SubflowFwdPackets",
        "SubflowFwdBytes",
        "SubflowBwdPackets",
        "SubflowBwdBytes",
        "ActDataPktFwd",
        "Protocol"
    };

    /// <summary>
    /// Features that may contain negative values or require mean normalization.
    /// </summary>
    public static readonly string[] ContinuousFeatures = new[]
    {
        "FlowDuration",
        "FwdPacketLengthMax",
        "FwdPacketLengthMin",
        "FwdPacketLengthMean",
        "FwdPacketLengthStd",
        "BwdPacketLengthMax",
        "BwdPacketLengthMin",
        "BwdPacketLengthMean",
        "BwdPacketLengthStd",
        "FlowBytesPerSecond",
        "FlowPacketsPerSecond",
        "FlowIATMean",
        "FlowIATStd",
        "FlowIATMax",
        "FlowIATMin",
        "FwdIATTotal",
        "FwdIATMean",
        "FwdIATStd",
        "FwdIATMax",
        "FwdIATMin",
        "BwdIATTotal",
        "BwdIATMean",
        "BwdIATStd",
        "BwdIATMax",
        "BwdIATMin",
        "FwdHeaderLength",
        "BwdHeaderLength",
        "FwdPacketsPerSecond",
        "BwdPacketsPerSecond",
        "MinPacketLength",
        "MaxPacketLength",
        "PacketLengthMean",
        "PacketLengthStd",
        "PacketLengthVariance",
        "DownUpRatio",
        "AveragePacketSize",
        "AvgFwdSegmentSize",
        "AvgBwdSegmentSize",
        "FwdAvgBytesPerBulk",
        "FwdAvgPacketsPerBulk",
        "FwdAvgBulkRate",
        "BwdAvgBytesPerBulk",
        "BwdAvgPacketsPerBulk",
        "BwdAvgBulkRate",
        "InitWinBytesForward",
        "InitWinBytesBackward",
        "ActiveMean",
        "ActiveStd",
        "ActiveMax",
        "ActiveMin",
        "IdleMean",
        "IdleStd",
        "IdleMax",
        "IdleMin",
        "MinSegSizeForward"
    };

    /// <summary>
    /// Gets the total number of features.
    /// </summary>
    public static int FeatureCount => NumericalFeatures.Length;

    /// <summary>
    /// Validates that all feature names are unique.
    /// </summary>
    /// <returns>True if all features are unique.</returns>
    public static bool ValidateUniqueFeatures()
    {
        var uniqueFeatures = new HashSet<string>(NumericalFeatures, StringComparer.OrdinalIgnoreCase);
        return uniqueFeatures.Count == NumericalFeatures.Length;
    }

    /// <summary>
    /// Gets a subset of features by indices.
    /// </summary>
    /// <param name="indices">The indices of features to retrieve.</param>
    /// <returns>Array of feature names.</returns>
    public static string[] GetFeaturesByIndices(params int[] indices)
    {
        return indices
            .Where(i => i >= 0 && i < NumericalFeatures.Length)
            .Select(i => NumericalFeatures[i])
            .ToArray();
    }

    /// <summary>
    /// Checks if a feature name exists in the configuration.
    /// </summary>
    /// <param name="featureName">The feature name to check.</param>
    /// <returns>True if the feature exists.</returns>
    public static bool ContainsFeature(string featureName)
    {
        return !string.IsNullOrWhiteSpace(featureName) &&
               NumericalFeatures.Contains(featureName, StringComparer.OrdinalIgnoreCase);
    }
}
