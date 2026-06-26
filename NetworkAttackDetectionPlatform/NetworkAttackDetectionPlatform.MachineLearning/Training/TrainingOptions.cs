namespace NetworkAttackDetectionPlatform.MachineLearning.Training
{
    public sealed class TrainingOptions
    {
        public int NumberOfTrees { get; init; } = 100;
        public int MaxDepth { get; init; } = 10;
        public double TestSplitRatio { get; init; } = 0.2;
        public int RandomSeed { get; init; } = 42;
    }
}
