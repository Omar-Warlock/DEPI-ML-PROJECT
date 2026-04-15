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

### Output files

1. [ML/data/processed/players_clean.csv](ML/data/processed/players_clean.csv)
2. [ML/data/features/model_ready.csv](ML/data/features/model_ready.csv)
3. [ML/data/processed/preprocessing_report.json](ML/data/processed/preprocessing_report.json)

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
