using System.Threading.Tasks;
using NetworkAttackDetectionPlatform.MachineLearning.Datasets;

namespace NetworkAttackDetectionPlatform.MachineLearning.Interfaces
{
    public interface IDatasetLoader
    {
        // Load a dataset from a local file path (CSV, Parquet etc.)
        Task<Dataset> LoadAsync(string path);
    }
}
