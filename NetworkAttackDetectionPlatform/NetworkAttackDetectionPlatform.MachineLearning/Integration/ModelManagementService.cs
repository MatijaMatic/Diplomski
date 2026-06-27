using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.Application.DTOs.ML;
using NetworkAttackDetectionPlatform.Application.Interfaces;
using NetworkAttackDetectionPlatform.MachineLearning.Models;
using NetworkAttackDetectionPlatform.MachineLearning.Training;
using NetworkAttackDetectionPlatform.MachineLearning.Utilities;

namespace NetworkAttackDetectionPlatform.MachineLearning.Integration
{
    public class ModelManagementService : IModelManagementService
    {
        private readonly IModelTrainer _trainer;
        private ModelInfo? _current;

        public ModelManagementService(IModelTrainer trainer)
        {
            _trainer = trainer;
        }

        public Task TrainModelAsync()
        {
            // Per requirements, do not train any model yet. Return completed task.
            return Task.CompletedTask;
        }

        public Task LoadModelAsync(string path)
        {
            // Do not load models in this phase; placeholder only
            return Task.CompletedTask;
        }

        public Task SaveModelAsync(string path)
        {
            // Do not save models in this phase; placeholder only
            return Task.CompletedTask;
        }

        public Task<ModelInfoDto?> GetCurrentModelInfoAsync()
        {
            if (_current == null)
            {
                return Task.FromResult<ModelInfoDto?>(new ModelInfoDto
                {
                    Name = "placeholder",
                    Version = "0.0",
                    TrainedAt = System.DateTime.UtcNow,
                    SizeBytes = 0
                });
            }

            var dto = new ModelInfoDto
            {
                Name = _current.Name,
                Version = _current.Version,
                TrainedAt = _current.TrainedAt,
                SizeBytes = _current.SizeBytes
            };

            return Task.FromResult<ModelInfoDto?>(dto);
        }
    }
}
