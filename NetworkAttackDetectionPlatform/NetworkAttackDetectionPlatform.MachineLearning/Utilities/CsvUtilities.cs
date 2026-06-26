using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NetworkAttackDetectionPlatform.MachineLearning.Utilities
{
    public static class CsvUtilities
    {
        public static async Task<IList<Dictionary<string, string>>> ReadCsvAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            var result = new List<Dictionary<string, string>>();
            if (!File.Exists(path)) return result;

            using var sr = new StreamReader(path);
            var header = await sr.ReadLineAsync().ConfigureAwait(false);
            if (header == null) return result;
            var columns = header.Split(',');

            while (!sr.EndOfStream)
            {
                var line = await sr.ReadLineAsync().ConfigureAwait(false);
                if (line == null) break;
                var parts = line.Split(',');
                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < columns.Length && i < parts.Length; i++)
                {
                    row[columns[i].Trim()] = parts[i].Trim();
                }
                result.Add(row);
            }

            return result;
        }
    }
}
