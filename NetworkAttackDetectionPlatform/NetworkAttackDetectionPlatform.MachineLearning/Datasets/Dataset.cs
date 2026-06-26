using System;
using System.Collections.Generic;

namespace NetworkAttackDetectionPlatform.MachineLearning.Datasets
{
    // Lightweight dataset container for ML workflows
    public sealed class Dataset
    {
        public string? Name { get; set; }
        public IReadOnlyList<Dictionary<string, object?>> Rows { get; init; } = Array.Empty<Dictionary<string, object?>>();
    }
}
