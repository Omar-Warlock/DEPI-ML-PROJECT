from __future__ import annotations

import argparse
from pathlib import Path

import numpy as np
import pandas as pd

try:
    from scipy.stats import skew as scipy_skew
except Exception:  # pragma: no cover - safe fallback if scipy is unavailable
    scipy_skew = None


def compute_skewness(series: pd.Series) -> float:
    valid = pd.to_numeric(series, errors="coerce").dropna()
    if valid.empty:
        raise ValueError("Cannot compute skewness on an empty column after numeric coercion.")

    if scipy_skew is not None:
        # Match pandas behavior reasonably by not enforcing unbiased correction.
        return float(scipy_skew(valid.to_numpy(), bias=True))

    return float(valid.skew())


def evaluate_log_transform(data_path: Path, target_col: str) -> None:
    if not data_path.exists():
        raise FileNotFoundError(f"Dataset not found: {data_path}")

    df = pd.read_csv(data_path)

    if target_col not in df.columns:
        raise ValueError(
            f"Column '{target_col}' not found in dataset. "
            f"Available columns: {sorted(df.columns.tolist())}"
        )

    # 1) Skewness before transformation.
    skew_before = compute_skewness(df[target_col])

    # 2) Log transformation using log1p to safely handle zeros.
    df["log_market_value"] = np.log1p(pd.to_numeric(df[target_col], errors="coerce"))

    # 3) Skewness after transformation.
    skew_after = compute_skewness(df["log_market_value"])

    print(f"Skewness Assessment for {target_col}:")
    print(f"--- Before Transformation: {skew_before:.2f}")
    print(f"--- After Log Transformation: {skew_after:.2f}")

    if abs(skew_after) < abs(skew_before):
        improvement = ((abs(skew_before) - abs(skew_after)) / abs(skew_before)) * 100
        print(
            f"Result: Skewness reduced by {improvement:.1f}%. "
            "The data is now more Gaussian-like."
        )
    else:
        print("Result: Transformation did not reduce skewness significantly.")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description=(
            "Evaluate how much log transformation reduces skewness for a target column."
        )
    )
    parser.add_argument(
        "--data-path",
        type=Path,
        default=Path("ML/data/processed/players_clean.csv"),
        help="Path to the CSV file (default: ML/data/processed/players_clean.csv)",
    )
    parser.add_argument(
        "--target-col",
        type=str,
        default="market_value_eur",
        help="Target numeric column to evaluate (default: market_value_eur)",
    )
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    evaluate_log_transform(args.data_path, args.target_col)


if __name__ == "__main__":
    main()
