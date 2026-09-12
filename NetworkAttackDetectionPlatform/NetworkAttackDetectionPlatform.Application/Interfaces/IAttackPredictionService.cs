using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.Application.DTOs.ML;

namespace NetworkAttackDetectionPlatform.Application.Interfaces
{
    public interface IAttackPredictionService
    {
        /// <summary>
        /// [DEPRECATED] Predict attack type/severity from 7 network traffic features.
        /// This is for demo/testing only and does NOT represent a valid ML prediction.
        /// </summary>
        Task<AttackClassificationResultDto> PredictAsync(NetworkTrafficDto traffic);

        /// <summary>
        /// [PREFERRED] Predict attack type/severity from all 78 CICIDS2017 network traffic features.
        /// This is the scientifically valid prediction method using the trained ML model.
        /// </summary>
        Task<AttackClassificationResultDto> PredictAsync(Cicids2017PredictionRequest traffic);
    }
}
