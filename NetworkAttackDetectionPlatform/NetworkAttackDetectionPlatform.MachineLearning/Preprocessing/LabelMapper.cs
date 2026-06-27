namespace NetworkAttackDetectionPlatform.MachineLearning.Preprocessing;

/// <summary>
/// Maps CICIDS2017 attack labels into normalized attack categories.
/// Provides deterministic, case-insensitive label transformation for ML training.
/// </summary>
public class LabelMapper
{
    private static readonly Dictionary<string, string> LabelMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        // Benign traffic
        ["BENIGN"] = "BENIGN",

        // DDoS attacks
        ["DDoS"] = "DDoS",

        // Port scanning
        ["PortScan"] = "PortScan",

        // Brute force attacks (FTP, SSH, Web)
        ["FTP-Patator"] = "BruteForce",
        ["SSH-Patator"] = "BruteForce",
        ["Web Attack - Brute Force"] = "BruteForce",
        ["Web Attack-Brute Force"] = "BruteForce",

        // Web attacks (XSS, SQL Injection)
        ["Web Attack - XSS"] = "WebAttack",
        ["Web Attack-XSS"] = "WebAttack",
        ["Web Attack - Sql Injection"] = "WebAttack",
        ["Web Attack-Sql Injection"] = "WebAttack",
        ["Web Attack - SQL Injection"] = "WebAttack",
        ["Web Attack-SQL Injection"] = "WebAttack",

        // Bot attacks
        ["Bot"] = "Bot",

        // DoS attacks (all variants mapped to DDoS for simplification)
        ["DoS Hulk"] = "DDoS",
        ["DoS GoldenEye"] = "DDoS",
        ["DoS Slowloris"] = "DDoS",
        ["DoS slowloris"] = "DDoS",
        ["DoS Slowhttptest"] = "DDoS",
        ["DoS slowhttptest"] = "DDoS",

        // Infiltration
        ["Infiltration"] = "Infiltration",

        // Heartbleed
        ["Heartbleed"] = "Other"
    };

    private static readonly HashSet<string> NormalizedCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        "BENIGN",
        "DDoS",
        "PortScan",
        "BruteForce",
        "Bot",
        "WebAttack",
        "Infiltration",
        "Other"
    };

    /// <summary>
    /// Maps a CICIDS2017 label to a normalized attack category.
    /// </summary>
    /// <param name="originalLabel">The original CICIDS2017 label.</param>
    /// <returns>The normalized attack category.</returns>
    public string MapLabel(string originalLabel)
    {
        if (string.IsNullOrWhiteSpace(originalLabel))
        {
            return "Other";
        }

        var trimmedLabel = originalLabel.Trim();

        if (LabelMappings.TryGetValue(trimmedLabel, out var mappedLabel))
        {
            return mappedLabel;
        }

        return "Other";
    }

    /// <summary>
    /// Maps a collection of labels to normalized categories.
    /// </summary>
    /// <param name="labels">The collection of original labels.</param>
    /// <returns>The collection of normalized labels.</returns>
    public IEnumerable<string> MapLabels(IEnumerable<string> labels)
    {
        return labels.Select(MapLabel);
    }

    /// <summary>
    /// Gets all supported normalized attack categories.
    /// </summary>
    /// <returns>A read-only set of normalized categories.</returns>
    public IReadOnlySet<string> GetNormalizedCategories()
    {
        return NormalizedCategories;
    }

    /// <summary>
    /// Checks if a label is a valid normalized category.
    /// </summary>
    /// <param name="label">The label to check.</param>
    /// <returns>True if the label is a valid normalized category.</returns>
    public bool IsNormalizedCategory(string label)
    {
        return !string.IsNullOrWhiteSpace(label) && 
               NormalizedCategories.Contains(label.Trim());
    }

    /// <summary>
    /// Gets the count of normalized categories.
    /// </summary>
    /// <returns>The number of distinct attack categories.</returns>
    public int GetCategoryCount()
    {
        return NormalizedCategories.Count;
    }

    /// <summary>
    /// Gets the original labels that map to a specific normalized category.
    /// </summary>
    /// <param name="normalizedCategory">The normalized category.</param>
    /// <returns>Collection of original labels that map to the category.</returns>
    public IEnumerable<string> GetOriginalLabelsForCategory(string normalizedCategory)
    {
        if (string.IsNullOrWhiteSpace(normalizedCategory))
        {
            return Enumerable.Empty<string>();
        }

        return LabelMappings
            .Where(kvp => kvp.Value.Equals(normalizedCategory, StringComparison.OrdinalIgnoreCase))
            .Select(kvp => kvp.Key);
    }
}
