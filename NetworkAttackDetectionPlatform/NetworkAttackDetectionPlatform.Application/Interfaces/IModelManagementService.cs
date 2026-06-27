using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.Application.DTOs.ML;

namespace NetworkAttackDetectionPlatform.Application.Interfaces
{
    public interface IModelManagementService
    {
        Task TrainModelAsync();
        Task LoadModelAsync(string path);
        Task SaveModelAsync(string path);
        Task<ModelInfoDto?> GetCurrentModelInfoAsync();
    }
}
