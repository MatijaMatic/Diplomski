namespace NetworkAttackDetectionPlatform.MachineLearning.Training;

public class TrainingOptions
{
    public string DatasetPath { get; set; } = string.Empty;

    public string ModelOutputPath { get; set; } = string.Empty;

    public int NumberOfTrees { get; set; } = 100;

    public int RandomSeed { get; set; } = 42;

    public double TestSplit { get; set; } = 0.2;
}