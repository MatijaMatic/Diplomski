using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.MachineLearning.Interfaces;
using NetworkAttackDetectionPlatform.MachineLearning.Utilities;

namespace NetworkAttackDetectionPlatform.MachineLearning.Datasets
{
    /// <summary>
    /// Implementation of IDatasetLoader that loads datasets from CSV files.
    /// Validates column count, skips invalid rows, and reports loading statistics.
    /// </summary>
    public class DatasetLoader : IDatasetLoader
    {
        private int _loadedSamplesCount;
        private int _skippedSamplesCount;

        /// <summary>
        /// Gets the number of samples successfully loaded in the last operation.
        /// </summary>
        public int LoadedSamplesCount => _loadedSamplesCount;

        /// <summary>
        /// Gets the number of samples skipped due to validation errors in the last operation.
        /// </summary>
        public int SkippedSamplesCount => _skippedSamplesCount;

        /// <summary>
        /// Loads a dataset from a CSV file with validation and error reporting.
        /// </summary>
        /// <param name="path">Path to the CSV file</param>
        /// <returns>A Dataset object containing the loaded data</returns>
        public async Task<Dataset> LoadAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path), "Dataset path cannot be null or empty.");

            if (!File.Exists(path))
                throw new FileNotFoundException($"Dataset file not found: {path}", path);

            _loadedSamplesCount = 0;
            _skippedSamplesCount = 0;

            var csvData = await CsvUtilities.ReadCsvAsync(path).ConfigureAwait(false);

            if (csvData.Count == 0)
            {
                throw new InvalidOperationException($"Dataset file is empty or has no valid rows: {path}");
            }

            // Get expected column count from first row (header is already consumed by CsvUtilities)
            var firstRow = csvData.FirstOrDefault();
            if (firstRow == null)
            {
                throw new InvalidOperationException("Dataset has no data rows.");
            }

            var expectedColumnCount = firstRow.Count;

            var rows = new List<Dictionary<string, object?>>();
            foreach (var row in csvData)
            {
                // Validate column count
                if (row.Count != expectedColumnCount)
                {
                    _skippedSamplesCount++;
                    continue; // Skip invalid rows
                }

                // Validate that row has at least some non-empty values
                if (!row.Values.Any(v => !string.IsNullOrWhiteSpace(v)))
                {
                    _skippedSamplesCount++;
                    continue; // Skip empty rows
                }

                var dictRow = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (var kvp in row)
                {
                    dictRow[kvp.Key] = kvp.Value;
                }
                rows.Add(dictRow);
                _loadedSamplesCount++;
            }

            if (_loadedSamplesCount == 0)
            {
                throw new InvalidOperationException($"No valid samples could be loaded from dataset: {path}");
            }

            return new Dataset
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Rows = rows.AsReadOnly()
            };
        }
    }
}
