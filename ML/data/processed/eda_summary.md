# FIFA EDA and Feature Validation Summary

Generated at (UTC): 2026-04-17T18:22:55.575029+00:00

## Dataset Overview
- players_clean rows/cols: 20621 / 16
- model_ready rows/cols: 20621 / 19

## Missingness
- players_clean total nulls: 0
- model_ready total nulls: 0

## Position Group Distribution
- ATT: 4053 (19.65%)
- DEF: 6870 (33.32%)
- GK: 2311 (11.21%)
- MID: 7387 (35.82%)

## Feature Checks
- age_potential_gap mismatches: 0
- age_potential_gap == 0 rows: 7056
- is_at_peak mismatches: 0
- is_at_peak share: 34.22%
- value_per_rating mismatches: 0
- value_per_rating non-finite rows: 0
- non-positive market_value_eur rows: 0
- low-value market_value_eur rows (<= 10000): 0
- one-hot missing columns: []
- one-hot invalid rows: 0
- position_group is one-hot encoded as: ['position_group_GK', 'position_group_DEF', 'position_group_MID', 'position_group_ATT']

## Strongest Correlations with log_value
- overall: 0.8852
- potential: 0.8384
- value_per_rating: 0.7553
- market_value_eur: 0.7363
- dribbling: 0.6055
- passing: 0.5912
- shooting: 0.3989
- physic: 0.3382
- pace: 0.2758
- defending: 0.2199

## Outlier Snapshot (IQR Method)
- market_value_eur: outliers=2274 (11.03%), bounds=[-1662500.00, 4037500.00]
- log_value: outliers=705 (3.42%), bounds=[10.99, 16.54]
- overall: outliers=184 (0.89%), bounds=[47.50, 83.50]
- potential: outliers=92 (0.45%), bounds=[52.50, 88.50]
- age: outliers=4 (0.02%), bounds=[9.00, 41.00]

For full details, see the paired JSON report.