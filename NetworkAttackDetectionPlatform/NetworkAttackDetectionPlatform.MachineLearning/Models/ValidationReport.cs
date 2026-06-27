namespace NetworkAttackDetectionPlatform.MachineLearning.Models;

/// <summary>
/// Represents a dataset validation report with quality metrics and diagnostics.
/// </summary>
public class ValidationReport
{
    /// <summary>
    /// Indicates whether the dataset passed all validation checks.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Total number of rows in the dataset.
    /// </summary>
    public int TotalRows { get; set; }

    /// <summary>
    /// Number of rows that passed validation.
    /// </summary>
    public int ValidRows { get; set; }

    /// <summary>
    /// Number of rows that failed validation.
    /// </summary>
    public int InvalidRows { get; set; }

    /// <summary>
    /// Total count of missing or null values across all features.
    /// </summary>
    public int MissingValueCount { get; set; }

    /// <summary>
    /// Number of rows with unknown or unsupported attack labels.
    /// </summary>
    public int UnknownLabelCount { get; set; }

    /// <summary>
    /// Collection of critical validation errors.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Collection of validation warnings that don't prevent training.
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Creates a summary string of the validation report.
    /// </summary>
    public string GetSummary()
    {
        return $"Validation: {(IsValid ? "PASSED" : "FAILED")} | " +
               $"Total: {TotalRows}, Valid: {ValidRows}, Invalid: {InvalidRows} | " +
               $"Missing Values: {MissingValueCount}, Unknown Labels: {UnknownLabelCount} | " +
               $"Errors: {Errors.Count}, Warnings: {Warnings.Count}";
    }
}
