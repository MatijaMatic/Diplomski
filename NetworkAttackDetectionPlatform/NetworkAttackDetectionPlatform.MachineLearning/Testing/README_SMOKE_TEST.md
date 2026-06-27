# CICIDS2017 Training Pipeline Smoke Test

## Purpose

This smoke test validates the complete end-to-end ML training pipeline for CICIDS2017 dataset integration.

## What It Tests

The smoke test verifies the following pipeline stages:

1. **Configuration Validation** - Ensures test configuration is valid
2. **Dataset Preparation** - Loads or generates sample CICIDS2017 data
3. **CSV Loading** - Tests `CsvDatasetReader` with 78 features
4. **Dataset Validation** - Runs `DatasetValidationService` quality checks
5. **Label Mapping** - Applies `LabelMapper` (15 ? 8 normalized classes)
6. **Preprocessing Pipeline** - Verifies `PreprocessingPipeline` builds correctly
7. **Model Training** - Executes FastTree training
8. **Model Output** - Verifies model and metadata files are created

## Running the Smoke Test

### Option 1: Using Generated Sample Data (Recommended for First Run)

The smoke test will automatically generate a sample CICIDS2017 dataset if no real data is available.

```bash
cd NetworkAttackDetectionPlatform.MachineLearning/Testing
dotnet run --project ../NetworkAttackDetectionPlatform.MachineLearning.csproj
```

This will:
- Generate 2,000 sample rows with realistic CICIDS2017 features
- Save to `./data/cicids2017_sample.csv`
- Train a FastTree model with 50 trees
- Save model to `./models/test_model.zip`

### Option 2: Using Real CICIDS2017 Dataset

If you have a real CICIDS2017 CSV file:

```bash
cd NetworkAttackDetectionPlatform.MachineLearning/Testing
dotnet run --project ../NetworkAttackDetectionPlatform.MachineLearning.csproj -- --dataset /path/to/cicids2017.csv --samples 5000
```

### Option 3: Programmatic Execution

```csharp
using NetworkAttackDetectionPlatform.MachineLearning.Testing;

// Create configuration
var config = Cicids2017TestConfiguration.CreateDefault();
config.SampleSize = 3000;
config.NumberOfTrees = 100;

// Run smoke test
var smokeTest = new Cicids2017TrainingSmokeTest(config);
var result = await smokeTest.RunAsync();

// Check result
if (result.Success)
{
    Console.WriteLine($"? Test passed! Accuracy: {result.TrainingResult.ValidationAccuracy:P2}");
}
else
{
    Console.WriteLine($"? Test failed: {result.Message}");
}
```

## Command Line Arguments

```
Usage: SmokeTestRunner [options]

Options:
  -d, --dataset <path>    Path to CICIDS2017 CSV file
  -s, --samples <count>   Number of samples to use (default: 2000)
  -t, --trees <count>     Number of trees for FastTree (default: 50)
  -o, --output <path>     Model output path (default: ./models/test_model.zip)
  -h, --help              Show help message
```

### Examples

```bash
# Default test (2000 samples, 50 trees)
dotnet run

# Custom sample size
dotnet run -- --samples 5000

# Using real dataset
dotnet run -- --dataset ./data/real_cicids2017.csv --samples 10000 --trees 100

# Quick test (fewer samples/trees)
dotnet run -- --samples 1000 --trees 25
```

## Expected Output

### Successful Run

```
???????????????????????????????????????????????????????????????
  CICIDS2017 TRAINING PIPELINE SMOKE TEST
???????????????????????????????????????????????????????????????

? 1. Configuration Validation

   ? Dataset Path: ./data/cicids2017_sample.csv
   ? Model Output Path: ./models/test_model.zip
   ? Sample Size: 2000 rows
   ? Expected Features: 78
   ? Number of Trees: 50
   ? Test Split: 20%
   ? Configuration is valid

? 2. Dataset Preparation

   ? Dataset file not found: ./data/cicids2017_sample.csv
   ? Generating sample CICIDS2017 dataset...
   ? Created directory: ./data
   ? Generated 2000 sample rows

? 3. CSV Dataset Loading

   ? CSV file loaded successfully
   ? Total rows: 2000
   ? Feature count: 78
   ? Unique labels found: 8
      - BENIGN
      - DDoS
      - PortScan
      - Bot
      - Web Attack - XSS
      - FTP-Patator
      - DoS Hulk
      - Infiltration

? 4. Dataset Quality Validation

   Validation: PASSED | Total: 2000, Valid: 2000, Invalid: 0 | ...
   ? Dataset validation passed

? 5. Label Mapping (15 ? 8 classes)

   Original label count: 8
   ? Label mapping applied
   Normalized label count: 8
   Normalized categories:
      - BENIGN: 267 samples (13.4%)
      - Bot: 241 samples (12.0%)
      - BruteForce: 221 samples (11.0%)
      - DDoS: 506 samples (25.3%)
      - Infiltration: 249 samples (12.4%)
      - Other: 0 samples (0.0%)
      - PortScan: 256 samples (12.8%)
      - WebAttack: 260 samples (13.0%)

? 6. Preprocessing Pipeline Verification

   ? Data loaded into ML.NET IDataView
   ? Schema validation passed
   ? Preprocessing pipeline built successfully
   Preprocessing Stats: 78 total features (78 numerical, 23 count, 55 continuous), 8 attack categories

? 7. Model Training Execution

   ? Training with 50 trees...
   ? Training completed successfully
   Training duration: 12.34s
   Training samples: 1600
   Validation samples: 400
   Accuracy: 85.25%
   Precision: 83.50%
   F1 Score: 0.8421
   Model path: ./models/test_model.zip
   Class labels: BENIGN, Bot, BruteForce, DDoS, Infiltration, Other, PortScan, WebAttack

? 8. Model Output Verification

   ? Model file created: ./models/test_model.zip
   Model size: 45,678 bytes (44.61 KB)
   ? Metadata file created: ./models/test_model.metadata.json

???????????????????????????????????????????????????????????????
  SMOKE TEST SUMMARY
???????????????????????????????????????????????????????????????

Status: ? PASSED
Total Duration: 15.67s
Training Duration: 12.34s

Dataset:
  - Total Rows: 2000
  - Original Labels: 8
  - Normalized Labels: 8
  - Generated: Yes

Training:
  - Training Samples: 1600
  - Validation Samples: 400
  - Accuracy: 85.25%
  - F1 Score: 0.8421

Model:
  - Path: ./models/test_model.zip
  - Size: 44.61 KB

????????????????????????????????????????????????????????????????
```

## Test Configuration

The test uses `Cicids2017TestConfiguration` with the following defaults:

- **Sample Size:** 2,000 rows
- **Number of Trees:** 50 (faster than production 100)
- **Test Split:** 20%
- **Random Seed:** 42 (reproducible results)
- **Validation:** Enabled
- **Label Mapping:** Enabled (15 ? 8 classes)
- **Normalization:** Enabled

## Generated Sample Data

If no real dataset is provided, the smoke test generates realistic sample data with:

- **78 CICIDS2017 features**
- **8 attack types:** BENIGN, DDoS, PortScan, Bot, Web Attack - XSS, FTP-Patator, DoS Hulk, Infiltration
- **Realistic feature distributions** based on attack type
- **CSV format** compatible with `CsvDatasetReader`

### Sample Data Characteristics

| Attack Type | Port | Packets | Duration | Bytes/sec |
|-------------|------|---------|----------|-----------|
| BENIGN | 80-443 | 5-50 | 1k-100k | 1k-10k |
| DDoS | 80 | 100-1k | 100-5k | 50k-500k |
| PortScan | Random | 1-5 | 10-100 | 100-1k |
| Bot | 1024+ | 10-100 | 10k-500k | 500-5k |
| BruteForce | 21-22 | 20-100 | 5k-50k | 2k-20k |
| WebAttack | 80 | 10-100 | 1k-50k | 5k-50k |

## Troubleshooting

### "Dataset validation failed"

**Cause:** Too many invalid rows or missing values  
**Solution:** Check `ValidationFailureThreshold` in configuration (default: 10%)

### "Training failed"

**Cause:** Insufficient data or invalid features  
**Solution:** Ensure at least 100 samples and all 78 features present

### "Model file not found"

**Cause:** Training completed but model save failed  
**Solution:** Check write permissions for `--output` directory

### Low accuracy (<50%)

**Cause:** Sample data is synthetic or insufficient samples  
**Solution:** Use real CICIDS2017 dataset or increase `--samples`

## Integration with CI/CD

The smoke test can be integrated into CI/CD pipelines:

```yaml
# .github/workflows/ml-pipeline-test.yml
name: ML Pipeline Smoke Test

on: [push, pull_request]

jobs:
  smoke-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '8.0.x'

      - name: Run smoke test
        run: |
          cd NetworkAttackDetectionPlatform.MachineLearning
          dotnet build
          dotnet run --project Testing/SmokeTestRunner.cs -- --samples 1000 --trees 25

      - name: Upload model artifact
        if: success()
        uses: actions/upload-artifact@v2
        with:
          name: test-model
          path: ./models/test_model.zip
```

## Next Steps

After smoke test passes:

1. ? **Phase 15D Complete** - ML pipeline verified
2. ?? **Phase 16** - API Integration (expose training endpoints)
3. ?? **Phase 17** - Blazor UI (training job management)
4. ?? **Phase 18** - Real CICIDS2017 dataset training
5. ?? **Phase 19** - Model performance optimization

## Files Created

- `Cicids2017TestConfiguration.cs` - Test configuration
- `Cicids2017SampleGenerator.cs` - Sample data generator
- `Cicids2017TrainingSmokeTest.cs` - Main smoke test
- `SmokeTestRunner.cs` - Console runner
- `README_SMOKE_TEST.md` - This file
