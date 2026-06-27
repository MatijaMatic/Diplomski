namespace NetworkAttackDetectionPlatform.MachineLearning.Testing;

/// <summary>
/// Configuration for CICIDS2017 dataset testing and validation.
/// </summary>
public class Cicids2017TestConfiguration
{
    /// <summary>
    /// Path to the CICIDS2017 CSV dataset file.
    /// </summary>
    public string DatasetPath { get; set; } = "./data/cicids2017_sample.csv";

    /// <summary>
    /// Path where the trained model will be saved during testing.
    /// </summary>
    public string ModelOutputPath { get; set; } = "./models/test_model.zip";

    /// <summary>
    /// Number of samples to use for smoke testing (use full dataset if 0).
    /// </summary>
    public int SampleSize { get; set; } = 2000;

    /// <summary>
    /// Expected number of features in CICIDS2017 dataset.
    /// </summary>
    public int ExpectedFeatureCount { get; set; } = 78;

    /// <summary>
    /// Expected normalized label categories after mapping.
    /// </summary>
    public string[] ExpectedNormalizedLabels { get; set; } = new[]
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
    /// Original CICIDS2017 labels that may appear in the dataset.
    /// </summary>
    public string[] ExpectedOriginalLabels { get; set; } = new[]
    {
        "BENIGN",
        "DDoS",
        "PortScan",
        "Bot",
        "Infiltration",
        "Web Attack - Brute Force",
        "Web Attack - XSS",
        "Web Attack - Sql Injection",
        "FTP-Patator",
        "SSH-Patator",
        "DoS Hulk",
        "DoS GoldenEye",
        "DoS Slowloris",
        "DoS Slowhttptest",
        "Heartbleed"
    };

    /// <summary>
    /// Number of trees for FastTree trainer during testing.
    /// </summary>
    public int NumberOfTrees { get; set; } = 50; // Reduced for faster testing

    /// <summary>
    /// Test/validation split ratio.
    /// </summary>
    public double TestSplit { get; set; } = 0.2;

    /// <summary>
    /// Random seed for reproducible testing.
    /// </summary>
    public int RandomSeed { get; set; } = 42;

    /// <summary>
    /// Whether to enable dataset validation.
    /// </summary>
    public bool ValidateDataset { get; set; } = true;

    /// <summary>
    /// Whether to apply label mapping.
    /// </summary>
    public bool ApplyLabelMapping { get; set; } = true;

    /// <summary>
    /// Whether to enable normalization.
    /// </summary>
    public bool EnableNormalization { get; set; } = true;

    /// <summary>
    /// Maximum acceptable percentage of invalid rows.
    /// </summary>
    public double ValidationFailureThreshold { get; set; } = 0.1; // 10%

    /// <summary>
    /// Validates the test configuration.
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(DatasetPath))
        {
            errorMessage = "Dataset path is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(ModelOutputPath))
        {
            errorMessage = "Model output path is required.";
            return false;
        }

        if (SampleSize < 0)
        {
            errorMessage = "Sample size must be non-negative.";
            return false;
        }

        if (ExpectedFeatureCount != 78)
        {
            errorMessage = $"Expected feature count should be 78 for CICIDS2017, got {ExpectedFeatureCount}.";
            return false;
        }

        if (NumberOfTrees < 1)
        {
            errorMessage = "Number of trees must be at least 1.";
            return false;
        }

        if (TestSplit <= 0 || TestSplit >= 1)
        {
            errorMessage = "Test split must be between 0 and 1.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Creates a default test configuration for quick smoke testing.
    /// </summary>
    public static Cicids2017TestConfiguration CreateDefault()
    {
        return new Cicids2017TestConfiguration
        {
            DatasetPath = "./data/cicids2017_sample.csv",
            ModelOutputPath = "./models/test_model.zip",
            SampleSize = 2000,
            NumberOfTrees = 50,
            TestSplit = 0.2,
            ValidateDataset = true,
            ApplyLabelMapping = true,
            EnableNormalization = true
        };
    }

    /// <summary>
    /// Creates a configuration for full dataset testing.
    /// </summary>
    public static Cicids2017TestConfiguration CreateFullDataset(string datasetPath)
    {
        return new Cicids2017TestConfiguration
        {
            DatasetPath = datasetPath,
            ModelOutputPath = "./models/full_model.zip",
            SampleSize = 0, // Use all rows
            NumberOfTrees = 100,
            TestSplit = 0.2,
            ValidateDataset = true,
            ApplyLabelMapping = true,
            EnableNormalization = true
        };
    }
}
