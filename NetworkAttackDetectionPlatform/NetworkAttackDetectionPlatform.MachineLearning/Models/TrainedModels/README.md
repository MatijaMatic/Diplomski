# Trained Models Directory

This directory stores trained ML.NET models and their associated metadata.

## File Structure

- `*.zip` - Trained ML.NET model files
- `*.metadata.json` - Model metadata files containing training metrics and configuration

## Usage

Models saved during training will be stored here. The training pipeline automatically creates and saves:
- Model file (e.g., `attack_classifier_v1.zip`)
- Metadata file (e.g., `attack_classifier_v1.metadata.json`)

## Notes

- Do not commit large model files to version control
- Add `*.zip` to `.gitignore` if models are large
- Keep metadata files for tracking model performance
