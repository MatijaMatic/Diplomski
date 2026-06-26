using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.MachineLearning.Datasets;

namespace NetworkAttackDetectionPlatform.MachineLearning.Training
{
    public interface IModelTrainer
    {
        // Train model using provided dataset and options. Should not persist by default.
        Task TrainAsync(Dataset dataset, TrainingOptions options);

        // Persist trained model to the given path
        Task SaveModelAsync(string path);

        // Load persisted model
        Task LoadModelAsync(string path);
    }
}
