namespace NetworkAttackDetectionPlatform.MachineLearning.Training;

public class TrainingOptions
{
    public string DatasetPath { get; set; } = string.Empty;

    public string ModelOutputPath { get; set; } = string.Empty;

    public int NumberOfTrees { get; set; } = 100;

    public int RandomSeed { get; set; } = 42;

    public double TestSplit { get; set; } = 0.2;

    // CICIDS2017-specific options
    public string DatasetName { get; set; } = "CICIDS2017";

    public string DatasetVersion { get; set; } = "1.0";

    public bool EnableNormalization { get; set; } = true;

    public bool ApplyLabelMapping { get; set; } = true;

    public bool ValidateDataset { get; set; } = true;

    public double ValidationFailureThreshold { get; set; } = 0.1; // 10% max invalid rows
}