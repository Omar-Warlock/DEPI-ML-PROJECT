# Log-Transform Skewness Evaluation Theory

## Why This Evaluation Exists
Financial targets like player market value are usually right-skewed:
- Most players have relatively low values.
- A small number of elite players have very large values.

This long right tail can make regression models harder to train and less stable.

## Core Idea
We compare skewness of the target before and after log transformation:
- Before: `market_value_eur`
- After: `log_market_value = log1p(market_value_eur)`

`log1p(x)` means `log(1 + x)`, which is preferred because:
- It is defined when `x = 0`.
- It compresses large values more than small ones.

## Formula Used In The Script
The script computes skewness using SciPy's moment-based Fisher-Pearson coefficient (`scipy.stats.skew` with `bias=True`):

`g1 = [ (1/n) * sum((xi - x_mean)^3) ] / [ ((1/n) * sum((xi - x_mean)^2)^(3/2)) ]`

Where:
- `n` is the number of observations.
- `xi` are the target values.
- `x_mean` is the sample mean.

## Skewness Interpretation
Skewness measures asymmetry of a distribution:
- Positive skewness: heavy right tail.
- Negative skewness: heavy left tail.
- Near zero: more symmetric (closer to Gaussian-like shape).

Evaluation rule used in the script:
- If `abs(skew_after) < abs(skew_before)`, transformation improved symmetry.
- Improvement percentage:

  `((abs(skew_before) - abs(skew_after)) / abs(skew_before)) * 100`

## Current Evaluation Output
Latest run output from `ML/scripts/evaluate_log_transform_skewness.py`:

```text
Skewness Assessment for market_value_eur:
--- Before Transformation: 4.53
--- After Log Transformation: 0.58
Result: Skewness reduced by 87.3%. The data is now more Gaussian-like.
```

## Formula Walkthrough With Real Values
Using the measured values:
- `skew_before = 4.53`
- `skew_after = 0.58`

Apply the improvement formula:

`((abs(skew_before) - abs(skew_after)) / abs(skew_before)) * 100`

Substitution:

`((abs(4.53) - abs(0.58)) / abs(4.53)) * 100`

`= ((4.53 - 0.58) / 4.53) * 100`

`= (3.95 / 4.53) * 100`

`= 87.3%` (rounded)

Explanation:
- The skewness moved from 4.53 (strong right-skew) to 0.58 (much closer to symmetric).
- This confirms that log transformation substantially reduced tail dominance.

## Why This Helps Modeling
Reducing skewness can improve:
- Numerical stability during optimization.
- Fit quality for models that work better with less extreme tails.
- Error behavior by reducing dominance of very large outliers.

## Practical Notes
- This check is diagnostic, not a strict pass/fail.
- Even after log transform, some outliers can remain (normal for market data).
- Always validate downstream model metrics before finalizing preprocessing.

## How To Run
From repository root:

```bash
/home/adham/Desktop/DEPI-ML-PROJECT/.venv/bin/python ML/scripts/evaluate_log_transform_skewness.py
```

Optional custom inputs:

```bash
/home/adham/Desktop/DEPI-ML-PROJECT/.venv/bin/python ML/scripts/evaluate_log_transform_skewness.py \
  --data-path ML/data/processed/players_clean.csv \
  --target-col market_value_eur
```
