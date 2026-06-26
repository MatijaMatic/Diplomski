using System.Collections.Generic;
using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.MachineLearning.Features;
using NetworkAttackDetectionPlatform.MachineLearning.Prediction;

namespace NetworkAttackDetectionPlatform.MachineLearning.Prediction
{
    public interface IPredictionService
    {
        Task<PredictionResult> PredictAsync(FeatureVector features);
        Task<IList<PredictionResult>> PredictBatchAsync(IEnumerable<FeatureVector> features);
    }
}
