using System.Threading.Tasks;
using Microsoft.ML;
using NetworkAttackDetectionPlatform.MachineLearning.Models;

namespace NetworkAttackDetectionPlatform.MachineLearning.Interfaces
{
    /// <summary>
    /// Defines the contract for saving trained ML models.
    /// </summary>
    public interface IModelSaver
    {
        /// <summary>
        /// Saves a trained model to the specified path.
        /// </summary>
        /// <param name="model">The trained ML.NET transformer model</param>
        /// <param name="schema">The data schema used for training</param>
        /// <param name="path">Path where the model should be saved</param>
        /// <returns>Task representing the save operation</returns>
        Task SaveModelAsync(ITransformer model, DataViewSchema schema, string path);

        /// <summary>
        /// Saves model metadata to accompany the trained model.
        /// </summary>
        /// <param name="metadata">Model metadata to save</param>
        /// <param name="path">Path where metadata should be saved</param>
        /// <returns>Task representing the save operation</returns>
        Task SaveMetadataAsync(ModelMetadata metadata, string path);
    }
}
