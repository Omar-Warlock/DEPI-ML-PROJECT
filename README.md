# DEPI-ML-PROJECT

## Data Preprocessing (Week 1-2)

This repo now includes a document-compliant preprocessing pipeline in [ML/scripts/preprocess_data.py](ML/scripts/preprocess_data.py).

### Required input files

1. FIFA 23 complete player CSV (required)
	Place the CSV directly under [ML/data/raw](ML/data/raw).
2. Transfermarkt archive (optional for later merge work)
	Already present as [ML/data/raw/transfer_market.zip](ML/data/raw/transfer_market.zip).

### Install dependencies

Run from the repository root:

```bash
/home/adham/Desktop/DEPI-ML-PROJECT/.venv/bin/python -m pip install -r ML/requirements.txt
```

### Run preprocessing

```bash
/home/adham/Desktop/DEPI-ML-PROJECT/.venv/bin/python ML/scripts/preprocess_data.py
```

### Run EDA and feature validation

```bash
/home/adham/Desktop/DEPI-ML-PROJECT/.venv/bin/python ML/scripts/generate_eda_report.py
```

### Interactive EDA notebook

1. Open [ML/notebooks/01_eda_feature_validation.ipynb](ML/notebooks/01_eda_feature_validation.ipynb).
2. Run cells top-to-bottom for interactive inspection of distributions, feature checks, correlations, and outlier summaries.

### Visual EDA notebook

1. Open [ML/notebooks/02_eda_visualizations.ipynb](ML/notebooks/02_eda_visualizations.ipynb).
2. Run cells top-to-bottom to generate charts (counts, distributions, boxplots, heatmap, scatter relationships).
3. Figures are saved under [ML/data/processed/figures](ML/data/processed/figures).

### Output files

1. [ML/data/processed/players_clean.csv](ML/data/processed/players_clean.csv)
2. [ML/data/features/model_ready.csv](ML/data/features/model_ready.csv)
3. [ML/data/processed/preprocessing_report.json](ML/data/processed/preprocessing_report.json)
4. [ML/data/processed/eda_report.json](ML/data/processed/eda_report.json)
5. [ML/data/processed/eda_summary.md](ML/data/processed/eda_summary.md)
6. [ML/data/processed/figures](ML/data/processed/figures)

### What the pipeline enforces

1. Drops columns with more than 40% missing values.
2. Median-imputes remaining numeric null values.
3. Parses and validates market value into numeric euros.
4. Filters invalid rows and clips extreme top-end outliers.
5. Engineers:
	- age_potential_gap
	- value_per_rating
	- one-hot encoded position groups (GK, DEF, MID, ATT)
6. Produces model-ready schema with zero null values and deterministic ordering.

### What EDA and feature validation checks

1. Dataset shape, missingness, and numeric distribution summaries.
2. Position-group frequency and per-group aggregate summaries.
3. IQR-based outlier snapshots for key numeric columns.
4. Correlations against log_value to surface strongest relationships.
5. Feature consistency checks for:
	- age_potential_gap derivation
	- value_per_rating derivation and finite values
	- one-hot position columns (presence and exactly-one validity)
	- range checks for age, overall, potential, and skill columns
