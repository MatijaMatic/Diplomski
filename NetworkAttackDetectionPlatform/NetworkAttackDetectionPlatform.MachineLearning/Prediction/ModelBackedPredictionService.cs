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
        private PredictionEngine<NetworkAttackDetectionPlatform.MachineLearning.Models.Cicids2017TrainingData, ModelOutput>? _predictionEngine;
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

                // Create prediction engine using the full 78-feature input schema
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<NetworkAttackDetectionPlatform.MachineLearning.Models.Cicids2017TrainingData, ModelOutput>(_transformer);

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
                    var input = MapToCicids2017TrainingData(features);
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

        private NetworkAttackDetectionPlatform.MachineLearning.Models.Cicids2017TrainingData MapToCicids2017TrainingData(FeatureVector fv)
        {
            if (fv == null || fv.Features == null)
                throw new ArgumentNullException(nameof(fv));

            var featuresList = NetworkAttackDetectionPlatform.MachineLearning.Preprocessing.FeatureConfiguration.NumericalFeatures;
            var dtoType = typeof(NetworkAttackDetectionPlatform.MachineLearning.Models.Cicids2017TrainingData);
            var obj = new NetworkAttackDetectionPlatform.MachineLearning.Models.Cicids2017TrainingData();

            for (int i = 0; i < featuresList.Length; i++)
            {
                var fname = featuresList[i];

                if (!fv.Features.TryGetValue(fname, out var raw) || raw == null)
                {
                    // Do not silently zero-fill required features for full 78-feature predictions
                    throw new ArgumentException($"Missing required feature '{fname}' for ML prediction.");
                }

                float floatVal;
                try
                {
                    floatVal = Convert.ToSingle(raw);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Feature '{fname}' could not be converted to float: {ex.Message}");
                }

                var prop = dtoType.GetProperty(fname);
                if (prop == null)
                {
                    throw new ArgumentException($"Cicids2017TrainingData missing property '{fname}'");
                }

                prop.SetValue(obj, floatVal);
            }

            // Label is not required for prediction
            obj.Label = string.Empty;
            return obj;
        }
    }
}
