using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ML;
using NetworkAttackDetectionPlatform.MachineLearning.Interfaces;

namespace NetworkAttackDetectionPlatform.MachineLearning.Utilities
{
    /// <summary>
    /// Implementation of IModelLoader for loading ML.NET models from disk.
    /// </summary>
    public class ModelLoader : IModelLoader
    {
        private readonly MLContext _mlContext;

        public ModelLoader(MLContext mlContext)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
        }

        /// <summary>
        /// Loads a trained model from the specified path.
        /// </summary>
        public async Task<ITransformer?> LoadModelAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                return null;

            return await Task.Run(() =>
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return _mlContext.Model.Load(stream, out _);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks if a model file exists at the specified path.
        /// </summary>
        public bool ModelExists(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
        }
    }
}
