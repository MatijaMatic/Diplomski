using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.ML.Data;

namespace NetworkAttackDetectionPlatform.MachineLearning.Evaluation
{
    /// <summary>
    /// Extracts and calculates correct multiclass classification metrics.
    /// ML.NET ConfusionMatrix provides per-class Counts as IReadOnlyList[IReadOnlyList[double]].
    /// </summary>
    public static class EvaluationMetricsExtractor
    {
        /// <summary>
        /// Extracts per-class metrics from a confusion matrix.
        /// </summary>
        public static PerClassMetrics[] ExtractPerClassMetrics(ConfusionMatrix confusionMatrix, string[] classLabels)
        {
            if (confusionMatrix == null)
                throw new ArgumentNullException(nameof(confusionMatrix));

            if (classLabels == null || classLabels.Length == 0)
                throw new ArgumentException("Class labels cannot be null or empty", nameof(classLabels));

            var results = new List<PerClassMetrics>();
            var numClasses = Math.Min(confusionMatrix.NumberOfClasses, classLabels.Length);

            for (int i = 0; i < numClasses; i++)
            {
                // TP: diagonal element (predicted i when actual is i)
                var tp = (long)confusionMatrix.Counts[i][i];

                // FP: sum of column i excluding diagonal (predicted i but actual is not i)
                var fp = 0L;
                for (int j = 0; j < numClasses; j++)
                {
                    if (i != j)
                        fp += (long)confusionMatrix.Counts[i][j];
                }

                // FN: sum of row i excluding diagonal (actual is i but predicted not i)
                var fn = 0L;
                for (int j = 0; j < numClasses; j++)
                {
                    if (i != j)
                        fn += (long)confusionMatrix.Counts[j][i];
                }

                // TN: sum of all elements except row i and column i
                var tn = 0L;
                for (int j = 0; j < numClasses; j++)
                {
                    for (int k = 0; k < numClasses; k++)
                    {
                        if (j != i && k != i)
                            tn += (long)confusionMatrix.Counts[j][k];
                    }
                }

                // Calculate metrics
                var precision = (tp + fp) > 0 ? (double)tp / (tp + fp) : 0.0;
                var recall = (tp + fn) > 0 ? (double)tp / (tp + fn) : 0.0;
                var f1 = (precision + recall) > 0 ? 2 * (precision * recall) / (precision + recall) : 0.0;
                var accuracy = (tp + tn) > 0 ? (double)(tp + tn) / (tp + tn + fp + fn) : 0.0;
                var support = tp + fn;

                var metric = new PerClassMetrics
                {
                    ClassName = i < classLabels.Length ? classLabels[i] : $"Class{i}",
                    ClassIndex = i,
                    Precision = precision,
                    Recall = recall,
                    F1Score = f1,
                    Accuracy = accuracy,
                    TruePositives = tp,
                    FalsePositives = fp,
                    FalseNegatives = fn,
                    TrueNegatives = tn,
                    Support = support
                };

                results.Add(metric);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Calculates aggregate metrics from per-class metrics.
        /// </summary>
        public static AggregateMetrics CalculateAggregateMetrics(
            PerClassMetrics[] perClassMetrics,
            double microAccuracy,
            double macroAccuracy)
        {
            if (perClassMetrics == null || perClassMetrics.Length == 0)
                throw new ArgumentException("Per-class metrics cannot be null or empty", nameof(perClassMetrics));

            // Macro averages: unweighted mean of per-class values
            var macroPrecision = perClassMetrics.Average(m => m.Precision);
            var macroRecall = perClassMetrics.Average(m => m.Recall);
            var macroF1 = perClassMetrics.Average(m => m.F1Score);

            // Weighted averages: weighted by support (number of true instances per class)
            var totalSupport = (double)perClassMetrics.Sum(m => m.Support);
            var weightedPrecision = totalSupport > 0
                ? perClassMetrics.Sum(m => m.Precision * m.Support) / totalSupport
                : 0.0;
            var weightedRecall = totalSupport > 0
                ? perClassMetrics.Sum(m => m.Recall * m.Support) / totalSupport
                : 0.0;
            var weightedF1 = totalSupport > 0
                ? perClassMetrics.Sum(m => m.F1Score * m.Support) / totalSupport
                : 0.0;

            return new AggregateMetrics
            {
                MicroAccuracy = microAccuracy,
                MacroAccuracy = macroAccuracy,
                MacroPrecision = macroPrecision,
                MacroRecall = macroRecall,
                MacroF1 = macroF1,
                WeightedPrecision = weightedPrecision,
                WeightedRecall = weightedRecall,
                WeightedF1 = weightedF1
            };
        }

        /// <summary>
        /// Serializes confusion matrix to JSON for storage.
        /// </summary>
        public static string SerializeConfusionMatrix(ConfusionMatrix matrix, string[] classLabels)
        {
            if (matrix == null)
                return "{}";

            var numClasses = Math.Min(matrix.NumberOfClasses, classLabels?.Length ?? 0);
            var data = new Dictionary<string, object>
            {
                ["NumberOfClasses"] = numClasses,
                ["Counts"] = SerializeMatrix(matrix.Counts, numClasses, classLabels)
            };

            return System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        private static object SerializeMatrix(IReadOnlyList<IReadOnlyList<double>> matrix, int numClasses, string[] classLabels)
        {
            var result = new List<Dictionary<string, object>>();

            for (int i = 0; i < numClasses; i++)
            {
                var row = new Dictionary<string, object>
                {
                    ["PredictedClass"] = classLabels != null && i < classLabels.Length ? classLabels[i] : $"Class{i}"
                };

                for (int j = 0; j < numClasses && j < matrix[i].Count; j++)
                {
                    var colName = classLabels != null && j < classLabels.Length ? classLabels[j] : $"Class{j}";
                    row[colName] = matrix[i][j];
                }

                result.Add(row);
            }

            return result;
        }
    }

    /// <summary>
    /// Metrics for a single class in multiclass classification.
    /// </summary>
    public class PerClassMetrics
    {
        [JsonPropertyName("className")]
        public string ClassName { get; set; } = string.Empty;

        [JsonPropertyName("classIndex")]
        public int ClassIndex { get; set; }

        [JsonPropertyName("precision")]
        public double Precision { get; set; }

        [JsonPropertyName("recall")]
        public double Recall { get; set; }

        [JsonPropertyName("f1Score")]
        public double F1Score { get; set; }

        [JsonPropertyName("accuracy")]
        public double Accuracy { get; set; }

        [JsonPropertyName("truePositives")]
        public long TruePositives { get; set; }

        [JsonPropertyName("falsePositives")]
        public long FalsePositives { get; set; }

        [JsonPropertyName("falseNegatives")]
        public long FalseNegatives { get; set; }

        [JsonPropertyName("trueNegatives")]
        public long TrueNegatives { get; set; }

        [JsonPropertyName("support")]
        public long Support { get; set; }

        public override string ToString() => 
            $"{ClassName}: P={Precision:F4} R={Recall:F4} F1={F1Score:F4} (TP={TruePositives} FP={FalsePositives} FN={FalseNegatives})";
    }

    /// <summary>
    /// Aggregate metrics across all classes.
    /// </summary>
    public class AggregateMetrics
    {
        [JsonPropertyName("microAccuracy")]
        public double MicroAccuracy { get; set; }

        [JsonPropertyName("macroAccuracy")]
        public double MacroAccuracy { get; set; }

        [JsonPropertyName("macroPrecision")]
        public double MacroPrecision { get; set; }

        [JsonPropertyName("macroRecall")]
        public double MacroRecall { get; set; }

        [JsonPropertyName("macroF1")]
        public double MacroF1 { get; set; }

        [JsonPropertyName("weightedPrecision")]
        public double WeightedPrecision { get; set; }

        [JsonPropertyName("weightedRecall")]
        public double WeightedRecall { get; set; }

        [JsonPropertyName("weightedF1")]
        public double WeightedF1 { get; set; }

        public override string ToString() =>
            $"Micro-Accuracy: {MicroAccuracy:P2} | Macro-P: {MacroPrecision:F4} R: {MacroRecall:F4} F1: {MacroF1:F4}";
    }
}
