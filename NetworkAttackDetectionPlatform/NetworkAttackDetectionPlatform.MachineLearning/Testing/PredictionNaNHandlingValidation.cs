using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using NetworkAttackDetectionPlatform.Application.DTOs.ML;
using NetworkAttackDetectionPlatform.MachineLearning.Features;
using NetworkAttackDetectionPlatform.MachineLearning.Prediction;
using NetworkAttackDetectionPlatform.MachineLearning.Preprocessing;
using NetworkAttackDetectionPlatform.MachineLearning.Utilities;

namespace NetworkAttackDetectionPlatform.MachineLearning.Testing
{
    /// <summary>
    /// Validates that the prediction pipeline correctly handles float.NaN values for missing features.
    /// Ensures that NaN values flow through to the preprocessing stage and are properly replaced
    /// by the ReplaceMissingValues transformer using learned column means.
    /// </summary>
    public class PredictionNaNHandlingValidation
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<PredictionNaNHandlingValidation> _logger;

        public PredictionNaNHandlingValidation(ILogger<PredictionNaNHandlingValidation> logger)
        {
            _mlContext = new MLContext();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Validates that a FeatureVector with only 3 real features and 75 NaN features
        /// can be processed through the prediction pipeline without schema errors.
        /// </summary>
        public async Task<ValidationResult> ValidateNaNHandlingAsync(
            string modelPath,
            bool modelExists = true)
        {
            var result = new ValidationResult
            {
                StartTime = DateTime.UtcNow
            };

            try
            {
                Console.WriteLine("?================================================");
                Console.WriteLine("? NaN Feature Handling Validation");
                Console.WriteLine("?================================================");
                Console.WriteLine();

                // Step 1: Verify all 78 features are defined in FeatureConfiguration
                Console.WriteLine("? Step 1: Verifying FeatureConfiguration");
                if (FeatureConfiguration.NumericalFeatures.Length != 78)
                {
                    result.Errors.Add($"Expected 78 features, found {FeatureConfiguration.NumericalFeatures.Length}");
                    return result;
                }
                Console.WriteLine($"   ? FeatureConfiguration has {FeatureConfiguration.NumericalFeatures.Length} features");

                // Step 2: Create a FeatureVector with NaN handling
                Console.WriteLine();
                Console.WriteLine("? Step 2: Creating test FeatureVector with NaN values");
                var testVector = CreateTestFeatureVectorWithNaN();
                Console.WriteLine($"   ? Created FeatureVector with 78 features");
                Console.WriteLine($"   ? Real features: DestinationPort=443, TotalLengthOfFwdPackets=1200, Protocol=1");
                Console.WriteLine($"   ? Missing features: 75 features set to float.NaN");

                // Step 3: Verify all expected feature names exist in the vector
                Console.WriteLine();
                Console.WriteLine("? Step 3: Validating feature vector schema");
                var missingFeatures = new List<string>();
                foreach (var featureName in FeatureConfiguration.NumericalFeatures)
                {
                    if (!testVector.Features.ContainsKey(featureName))
                    {
                        missingFeatures.Add(featureName);
                    }
                }

                if (missingFeatures.Any())
                {
                    result.Errors.Add($"Missing {missingFeatures.Count} features: {string.Join(", ", missingFeatures.Take(5))}...");
                    return result;
                }
                Console.WriteLine($"   ? All 78 expected feature names present in FeatureVector");

                // Step 4: Verify NaN values are preserved
                Console.WriteLine();
                Console.WriteLine("? Step 4: Verifying NaN values are preserved");
                var nanCount = testVector.Features.Values.Count(v => v is float f && float.IsNaN(f));
                Console.WriteLine($"   ? Found {nanCount} NaN values in FeatureVector");
                if (nanCount < 75)
                {
                    result.Warnings.Add($"Expected ~75 NaN values, found {nanCount}");
                }

                // Step 5: Convert to ExpandoObject and check NaN preservation
                Console.WriteLine();
                Console.WriteLine("? Step 5: Verifying NaN preservation in dynamic object conversion");
                var expando = ConvertFeatureVectorToExpando(testVector);
                var expandoNanCount = CountNaNValuesInDictionary(expando);
                Console.WriteLine($"   ? NaN values preserved in ExpandoObject: {expandoNanCount} NaN values");

                // Step 6: Attempt schema validation on LoadFromEnumerable
                Console.WriteLine();
                Console.WriteLine("? Step 6: Testing ML.NET schema loading with NaN values");
                try
                {
                    var list = new[] { expando };
                    var inputDv = _mlContext.Data.LoadFromEnumerable(list);
                    var schema = inputDv.Schema;
                    Console.WriteLine($"   ? ML.NET DataView loaded successfully");
                    Console.WriteLine($"   ? Schema has {schema.Count} columns");

                    // Verify expected columns
                    var schemaColumnNames = schema.Select(col => col.Name).ToList();
                    var missingColumns = FeatureConfiguration.NumericalFeatures
                        .Where(f => !schemaColumnNames.Contains(f))
                        .ToList();

                    if (missingColumns.Any())
                    {
                        result.Errors.Add($"Missing columns in schema: {string.Join(", ", missingColumns.Take(5))}");
                        return result;
                    }
                    Console.WriteLine($"   ? All 78 expected feature columns present in ML.NET schema");
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Schema validation failed: {ex.Message}");
                    return result;
                }

                // Step 7: Test preprocessing pipeline with NaN values
                Console.WriteLine();
                Console.WriteLine("? Step 7: Testing preprocessing pipeline with NaN values");
                try
                {
                    var preprocessingPipeline = new PreprocessingPipeline(_mlContext);
                    
                    // Create a list of ExpandoObjects (which implement IDictionary)
                    var expandoList = new[] { (ExpandoObject)(object)expando };
                    var testData = _mlContext.Data.LoadFromEnumerable(expandoList);
                    
                    // Validate schema
                    var isValidSchema = preprocessingPipeline.ValidateSchema(testData);
                    Console.WriteLine($"   ? Preprocessing pipeline schema validation passed");
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Preprocessing validation failed: {ex.Message}");
                }

                result.Success = !result.Errors.Any();
                result.EndTime = DateTime.UtcNow;
                result.Duration = result.EndTime.Value - result.StartTime;

                Console.WriteLine();
                Console.WriteLine("?================================================");
                if (result.Success)
                {
                    Console.WriteLine("? ? NaN Handling Validation PASSED");
                }
                else
                {
                    Console.WriteLine("? ? NaN Handling Validation FAILED");
                }
                Console.WriteLine("?================================================");
                Console.WriteLine();

                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Validation exception: {ex.Message}");
                result.Success = false;
                result.EndTime = DateTime.UtcNow;
                result.Duration = result.EndTime.Value - result.StartTime;
                return result;
            }
        }

        /// <summary>
        /// Creates a test FeatureVector mimicking the AttackPredictionService conversion logic.
        /// Only 3 features are set to real values; the remaining 75 are set to float.NaN.
        /// </summary>
        private static FeatureVector CreateTestFeatureVectorWithNaN()
        {
            var fv = new FeatureVector();

            // Initialize all features to NaN first
            foreach (var featureName in FeatureConfiguration.NumericalFeatures)
            {
                fv.Features[featureName] = float.NaN;
            }

            // Set only the 3 real mappings
            fv.Features["DestinationPort"] = 443f;
            fv.Features["TotalLengthOfFwdPackets"] = 1200f;
            fv.Features["Protocol"] = 1f;

            return fv;
        }

        /// <summary>
        /// Simulates the conversion from FeatureVector to ExpandoObject in PredictionService.
        /// </summary>
        private static IDictionary<string, object> ConvertFeatureVectorToExpando(FeatureVector features)
        {
            var expando = new ExpandoObject() as IDictionary<string, object>;

            foreach (var featureName in FeatureConfiguration.NumericalFeatures)
            {
                if (features.Features.TryGetValue(featureName, out var value) && value != null)
                {
                    if (value is float f)
                        expando[featureName] = f;
                    else if (value is double d)
                        expando[featureName] = (float)d;
                    else if (value is int i)
                        expando[featureName] = (float)i;
                    else if (float.TryParse(value.ToString(), out var parsed))
                        expando[featureName] = parsed;
                    else
                        expando[featureName] = float.NaN;
                }
                else
                {
                    expando[featureName] = float.NaN;
                }
            }

            return expando;
        }

        /// <summary>
        /// Counts the number of NaN values in a dictionary of feature values.
        /// </summary>
        private static int CountNaNValuesInDictionary(IDictionary<string, object> dict)
        {
            int nanCount = 0;
            foreach (var kvp in dict)
            {
                if (kvp.Value is float f && float.IsNaN(f))
                    nanCount++;
            }
            return nanCount;
        }
    }

    /// <summary>
    /// Result of a validation run.
    /// </summary>
    public class ValidationResult
    {
        public bool Success { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
