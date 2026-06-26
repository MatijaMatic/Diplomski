using System.Collections.Generic;

namespace NetworkAttackDetectionPlatform.MachineLearning.Features
{
    // Represents engineered features for a single example used for prediction
    public sealed class FeatureVector
    {
        public Dictionary<string, object?> Features { get; init; } = new();
    }
}
