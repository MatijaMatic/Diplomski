using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.MachineLearning.Training;

namespace NetworkAttackDetectionPlatform.MachineLearning.Interfaces
{
    /// <summary>
    /// Defines the contract for orchestrating the complete ML training pipeline.
    /// Responsible for loading data, preprocessing, training, evaluation, and model persistence.
    /// </summary>
    public interface ITrainingPipeline
    {
        /// <summary>
        /// Executes the complete training pipeline with the specified options.
        /// </summary>
        /// <param name="options">Training configuration options</param>
        /// <returns>Training result containing metrics and metadata</returns>
        Task<TrainingResult> ExecuteAsync(TrainingOptions options);
    }
}
