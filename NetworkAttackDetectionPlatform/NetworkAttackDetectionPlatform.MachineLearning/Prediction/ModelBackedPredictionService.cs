using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using NetworkAttackDetectionPlatform.MachineLearning.Features;
using NetworkAttackDetectionPlatform.MachineLearning.Interfaces;
using NetworkAttackDetectionPlatform.MachineLearning.Models;

namespace NetworkAttackDetectionPlatform.MachineLearning.Prediction
{
    /// <summary>
    /// Attempts to load a persisted ML.NET model and use it for predictions.
    /// Falls back to the deterministic `PredictionService` when no model is available or loading/prediction fails.
    /// Logs which provider is active to make the choice observable.
    /// </summary>
    public class ModelBackedPredictionService : IPredictionService
    {
        private readonly MLContext _mlContext;
        private readonly IModelLoader _modelLoader;
        private readonly PredictionService _fallback;
        private readonly string _modelPath;

        // If a real model is loaded, store transformer and a prediction engine
        private ITransformer? _transformer;
        private PredictionEngine<ModelInput, ModelOutput>? _predictionEngine;
        private int _featureVectorSize = 0;
        private bool _useModel = false;

        public ModelBackedPredictionService(
            MLContext mlContext,
            IModelLoader modelLoader,
            PredictionService fallback)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _modelLoader = modelLoader ?? throw new ArgumentNullException(nameof(modelLoader));
            _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));

            // Default expected location relative to application base
            _modelPath = Path.Combine(AppContext.BaseDirectory, "MachineLearning", "Models", "TrainedModels", "attack_classifier.zip");

            TryLoadModel();
        }

        private void TryLoadModel()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_modelPath) || !_modelLoader.ModelExists(_modelPath))
                {
                    PrintModelNotFoundWarning(_modelPath);
                    _useModel = false;
                    return;
                }

                // Load transformer
                _transformer = _modelLoader.LoadModelAsync(_modelPath).GetAwaiter().GetResult();
                if (_transformer == null)
                {
                    PrintModelLoadFailureWarning(_modelPath);
                    _useModel = false;
                    return;
                }

                // Also load schema to determine expected feature vector size
                using var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var loaded = _mlContext.Model.Load(stream, out var modelSchema);
                // Find the Features column and its vector size
                for (int i = 0; i < modelSchema.Count; i++)
                {
                    var col = modelSchema[i];
                    if (string.Equals(col.Name, "Features", StringComparison.OrdinalIgnoreCase))
                    {
                        if (col.Type is VectorDataViewType vdt)
                        {
                            _featureVectorSize = (int)vdt.Dimensions[0];
                        }
                        break;
                    }
                }

                // If feature vector size is unknown, fall back to FeatureConfiguration.FeatureCount when available
                if (_featureVectorSize <= 0)
                {
                    try
                    {
                        _featureVectorSize = NetworkAttackDetectionPlatform.MachineLearning.Preprocessing.FeatureConfiguration.FeatureCount;
                    }
                    catch
                    {
                        _featureVectorSize = 0;
                    }
                }

                // Create prediction engine
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_transformer);

                _useModel = true;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("??????????????????????????????????????????????????????");
                Console.WriteLine("?   ? ML MODEL LOADED SUCCESSFULLY                   ?");
                Console.WriteLine("??????????????????????????????????????????????????????");
                Console.WriteLine($"?  Model Path: {_modelPath}");
                Console.WriteLine($"?  Feature Count: {_featureVectorSize}");
                Console.WriteLine("?  Status: USING TRAINED ML MODEL FOR PREDICTIONS   ?");
                Console.WriteLine("?  This is scientifically valid and suitable for    ?");
                Console.WriteLine("?  thesis documentation.                            ?");
                Console.WriteLine("??????????????????????????????????????????????????????");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                PrintModelLoadFailureWarning(_modelPath);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"?  Details: {ex.Message}");
                Console.ResetColor();
                _useModel = false;
                _transformer = null;
                _predictionEngine = null;
            }
        }

        private static void PrintModelNotFoundWarning(string path)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("??????????????????????????????????????????????????????");
            Console.WriteLine("?   ??  ML MODEL NOT FOUND - DEVELOPMENT FALLBACK    ?");
            Console.WriteLine("??????????????????????????????????????????????????????");
            Console.WriteLine($"?  Expected Path: {path}");
            Console.WriteLine("?                                                    ?");
            Console.WriteLine("?  Predictions will use DETERMINISTIC HASHING        ?");
            Console.WriteLine("?  This is NOT a trained ML model.                  ?");
            Console.WriteLine("?                                                    ?");
            Console.WriteLine("?  For scientifically valid predictions:            ?");
            Console.WriteLine("?  1. Obtain real CICIDS2017 dataset                ?");
            Console.WriteLine("?  2. Run training pipeline                         ?");
            Console.WriteLine("?  3. Copy model to expected path                   ?");
            Console.WriteLine("?  4. Restart application                           ?");
            Console.WriteLine("?                                                    ?");
            Console.WriteLine("?  Until then, predictions are NOT suitable for     ?");
            Console.WriteLine("?  thesis results or production use.                ?");
            Console.WriteLine("??????????????????????????????????????????????????????");
            Console.ResetColor();
        }

        private static void PrintModelLoadFailureWarning(string path)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("??????????????????????????????????????????????????????");
            Console.WriteLine("?   ? ML MODEL LOAD FAILED - DEVELOPMENT FALLBACK   ?");
            Console.WriteLine("??????????????????????????????????????????????????????");
            Console.WriteLine($"?  Path: {path}");
            Console.WriteLine("?                                                    ?");
            Console.WriteLine("?  Using non-ML deterministic hashing for fallback.?");
            Console.WriteLine("?  Results are NOT suitable for thesis documentation ?");
            Console.WriteLine("?  or scientific evaluation.                        ?");
            Console.WriteLine("??????????????????????????????????????????????????????");
            Console.ResetColor();
        }

        public Task<PredictionResult> PredictAsync(FeatureVector features)
        {
            if (_useModel && _predictionEngine != null)
            {
                try
                {
                    var input = MapToModelInput(features);
                    var output = _predictionEngine.Predict(input);

                    // Derive a confidence value. Prefer Probability if present, otherwise compute softmax over Score.
                    double confidence = 0.0;
                    if (output != null)
                    {
                        if (output.Probability > 0.0f)
                        {
                            confidence = Math.Clamp(output.Probability * 100.0, 0.0, 100.0);
                        }
                        else if (output.Score != null && output.Score.Length > 0)
                        {
                            var scores = output.Score.Select(s => (double)s).ToArray();
                            var max = scores.Max();
                            var exps = scores.Select(s => Math.Exp(s - max)).ToArray();
                            var sum = exps.Sum();
                            var softmax = exps.Select(e => e / sum).ToArray();
                            var maxi = Array.IndexOf(exps, exps.Max());
                            confidence = Math.Clamp(softmax[maxi] * 100.0, 0.0, 100.0);
                        }
                    }

                    var result = new PredictionResult
                    {
                        Label = output?.PredictedLabel ?? string.Empty,
                        Score = confidence,
                        IsAnomaly = confidence > 80.0
                    };

                    return Task.FromResult(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ML] Model-backed prediction failed: {ex.Message}. Falling back to deterministic prediction.");
                    // disable further attempts to avoid repeated failures
                    _useModel = false;
                }
            }

            // Fallback deterministic
            return _fallback.PredictAsync(features);
        }

        public Task<IList<PredictionResult>> PredictBatchAsync(IEnumerable<FeatureVector> features)
        {
            // Simple batch implementation using single predict
            var list = features.Select(f => PredictAsync(f).GetAwaiter().GetResult()).ToList();
            return Task.FromResult((IList<PredictionResult>)list);
        }

        private ModelInput MapToModelInput(FeatureVector fv)
        {
            var arrSize = Math.Max(0, _featureVectorSize);
            if (arrSize == 0)
            {
                // Fallback size of 9 (legacy ModelInput) to avoid nulls
                arrSize = 9;
            }

            var input = new ModelInput
            {
                Features = new float[arrSize],
                Label = string.Empty
            };

            // Best-effort: map a few well-known values if present in the feature vector into the expected indices.
            // The authoritative order is defined by FeatureConfiguration.NumericalFeatures during training.
            try
            {
                var featuresList = NetworkAttackDetectionPlatform.MachineLearning.Preprocessing.FeatureConfiguration.NumericalFeatures;
                for (int i = 0; i < Math.Min(arrSize, featuresList.Length); i++)
                {
                    var fname = featuresList[i];
                    if (fv?.Features != null && fv.Features.TryGetValue(fname, out var val) && val != null)
                    {
                        if (float.TryParse(val.ToString(), out var f))
                        {
                            input.Features[i] = f;
                        }
                    }
                }

                // Also map common names that might be present in incoming FeatureVector
                if (fv?.Features != null)
                {
                    if (fv.Features.TryGetValue("SourcePort", out var sp) && sp != null)
                    {
                        var idx = Array.IndexOf(featuresList, "SourcePort");
                        if (idx >= 0 && idx < input.Features.Length && float.TryParse(sp.ToString(), out var fsp))
                            input.Features[idx] = fsp;
                    }

                    if (fv.Features.TryGetValue("DestinationPort", out var dp) && dp != null)
                    {
                        var idx = Array.IndexOf(featuresList, "DestinationPort");
                        if (idx >= 0 && idx < input.Features.Length && float.TryParse(dp.ToString(), out var fdp))
                            input.Features[idx] = fdp;
                    }

                    if (fv.Features.TryGetValue("Protocol", out var pr) && pr != null)
                    {
                        var idx = Array.IndexOf(featuresList, "Protocol");
                        if (idx >= 0 && idx < input.Features.Length && float.TryParse(pr.ToString(), out var fpr))
                            input.Features[idx] = fpr;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ML] Non-fatal: mapping to ModelInput failed for some features; zeros will be used. {ex.Message}");
            }

            return input;
        }
    }
}
