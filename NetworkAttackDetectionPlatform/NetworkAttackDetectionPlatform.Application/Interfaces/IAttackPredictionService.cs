using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.Application.DTOs.ML;

namespace NetworkAttackDetectionPlatform.Application.Interfaces
{
    public interface IAttackPredictionService
    {
        // Predict attack type/severity from network traffic features
        Task<AttackClassificationResultDto> PredictAsync(NetworkTrafficDto traffic);
    }
}
