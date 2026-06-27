using System.Threading.Tasks;
using Microsoft.ML;

namespace NetworkAttackDetectionPlatform.MachineLearning.Interfaces
{
    /// <summary>
    /// Defines the contract for loading trained ML models.
    /// </summary>
    public interface IModelLoader
    {
        /// <summary>
        /// Loads a trained model from the specified path.
        /// </summary>
        /// <param name="path">Path to the model file</param>
        /// <returns>The loaded ML.NET transformer model</returns>
        Task<ITransformer?> LoadModelAsync(string path);

        /// <summary>
        /// Checks if a model file exists at the specified path.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if model exists, false otherwise</returns>
        bool ModelExists(string path);
    }
}
