using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.ML;
using NetworkAttackDetectionPlatform.MachineLearning.Interfaces;
using NetworkAttackDetectionPlatform.MachineLearning.Models;

namespace NetworkAttackDetectionPlatform.MachineLearning.Utilities
{
    /// <summary>
    /// Implementation of IModelSaver for saving ML.NET models and metadata to disk.
    /// </summary>
    public class ModelSaver : IModelSaver
    {
        private readonly MLContext _mlContext;

        public ModelSaver(MLContext mlContext)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
        }

        /// <summary>
        /// Saves a trained model to the specified path.
        /// </summary>
        public async Task SaveModelAsync(ITransformer model, DataViewSchema schema, string path)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            // Ensure directory exists
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await Task.Run(() =>
            {
                using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                _mlContext.Model.Save(model, schema, stream);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves model metadata to accompany the trained model.
        /// </summary>
        public async Task SaveMetadataAsync(ModelMetadata metadata, string path)
        {
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            // Ensure directory exists
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(path, json).ConfigureAwait(false);
        }
    }
}
