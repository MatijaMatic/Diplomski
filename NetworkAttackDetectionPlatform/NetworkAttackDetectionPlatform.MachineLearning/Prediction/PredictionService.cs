using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.MachineLearning.Features;

namespace NetworkAttackDetectionPlatform.MachineLearning.Prediction
{
    public class PredictionService : IPredictionService
    {
        // Deterministic dummy prediction implementation based on available features
        public Task<PredictionResult> PredictAsync(FeatureVector features)
        {
            double score = 0.0;
            bool isAnomaly = false;
            string label = "Normal";

            if (features?.Features != null)
            {
                // Prefer PayloadSize if available
                if (features.Features.TryGetValue("PayloadSize", out var payloadObj) && payloadObj != null)
                {
                    if (payloadObj is int payloadInt)
                    {
                        score = (double)(payloadInt % 101); // 0..100
                    }
                    else if (int.TryParse(payloadObj.ToString(), out var p))
                    {
                        score = (double)(p % 101);
                    }
                }
                else if (features.Features.TryGetValue("SourceIp", out var ipObj) && ipObj != null)
                {
                    // Fallback: stable hash-based score
                    var s = ipObj.ToString() ?? string.Empty;
                    score = (double)(System.Math.Abs(s.GetHashCode()) % 101);
                }
            }

            isAnomaly = score > 80.0;
            label = isAnomaly ? "DDoS" : "Normal";

            var result = new PredictionResult
            {
                Label = label,
                Score = score,
                IsAnomaly = isAnomaly
            };

            return Task.FromResult(result);
        }

        public Task<IList<PredictionResult>> PredictBatchAsync(IEnumerable<FeatureVector> features)
        {
            var list = features.Select(f => PredictAsync(f).GetAwaiter().GetResult()).ToList();
            return Task.FromResult((IList<PredictionResult>)list);
        }
    }
}
