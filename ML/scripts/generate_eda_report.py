from __future__ import annotations

import json
from dataclasses import dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Dict, List

import numpy as np
import pandas as pd


@dataclass
class EDAPaths:
    players_clean_path: Path
    model_ready_path: Path
    report_json_path: Path
    report_markdown_path: Path


REQUIRED_PLAYERS_CLEAN_COLUMNS = [
    "short_name",
    "age",
    "overall",
    "potential",
    "position_group",
    "market_value_eur",
    "pace",
    "shooting",
    "passing",
    "dribbling",
    "defending",
    "physic",
    "age_potential_gap",
    "is_at_peak",
    "value_per_rating",
]

POSITION_DUMMY_COLUMNS = [
    "position_group_GK",
    "position_group_DEF",
    "position_group_MID",
    "position_group_ATT",
]


def require_columns(df: pd.DataFrame, required_columns: List[str], label: str) -> None:
    missing = [col for col in required_columns if col not in df.columns]
    if missing:
        raise ValueError(f"{label} is missing required columns: {missing}")


def to_float(value: object) -> float:
    return float(np.asarray(value, dtype=float))


def summarize_numeric(df: pd.DataFrame) -> Dict[str, Dict[str, float]]:
    numeric_cols = df.select_dtypes(include=[np.number]).columns.tolist()
    summary: Dict[str, Dict[str, float]] = {}
    for col in numeric_cols:
        series = df[col].dropna()
        if series.empty:
            continue
        summary[col] = {
            "count": int(series.shape[0]),
            "mean": to_float(series.mean()),
            "std": to_float(series.std(ddof=1)) if series.shape[0] > 1 else 0.0,
            "min": to_float(series.min()),
            "p25": to_float(series.quantile(0.25)),
            "p50": to_float(series.quantile(0.50)),
            "p75": to_float(series.quantile(0.75)),
            "max": to_float(series.max()),
            "skew": to_float(series.skew()) if series.shape[0] > 2 else 0.0,
        }
    return summary


def summarize_missingness(df: pd.DataFrame) -> Dict[str, object]:
    null_counts = df.isna().sum()
    total_nulls = int(null_counts.sum())
    rows, cols = df.shape
    null_share = float(total_nulls / (rows * cols)) if rows and cols else 0.0
    columns_with_nulls = {
        col: {
            "null_count": int(count),
            "null_ratio": float(count / rows) if rows else 0.0,
        }
        for col, count in null_counts.items()
        if count > 0
    }
    return {
        "total_nulls": total_nulls,
        "overall_null_share": null_share,
        "columns_with_nulls": columns_with_nulls,
    }


def iqr_outlier_summary(series: pd.Series) -> Dict[str, float]:
    valid = series.dropna()
    if valid.empty:
        return {
            "q1": 0.0,
            "q3": 0.0,
            "iqr": 0.0,
            "lower_bound": 0.0,
            "upper_bound": 0.0,
            "outlier_count": 0,
            "outlier_share": 0.0,
        }

    q1 = valid.quantile(0.25)
    q3 = valid.quantile(0.75)
    iqr = q3 - q1
    lower_bound = q1 - 1.5 * iqr
    upper_bound = q3 + 1.5 * iqr
    outliers = valid[(valid < lower_bound) | (valid > upper_bound)]

    return {
        "q1": to_float(q1),
        "q3": to_float(q3),
        "iqr": to_float(iqr),
        "lower_bound": to_float(lower_bound),
        "upper_bound": to_float(upper_bound),
        "outlier_count": int(outliers.shape[0]),
        "outlier_share": float(outliers.shape[0] / valid.shape[0]),
    }


def compute_feature_checks(players_clean: pd.DataFrame, model_ready: pd.DataFrame) -> Dict[str, object]:
    age_gap_expected = players_clean["potential"] - players_clean["overall"]
    age_gap_mismatch_rows = int((players_clean["age_potential_gap"] != age_gap_expected).sum())
    age_gap_zero_rows = int((players_clean["age_potential_gap"] == 0).sum())

    expected_is_at_peak = (players_clean["age_potential_gap"] == 0).astype(int)
    is_at_peak_mismatch_rows = int((players_clean["is_at_peak"] != expected_is_at_peak).sum())
    is_at_peak_share = float(players_clean["is_at_peak"].mean()) if len(players_clean) else 0.0

    expected_value_per_rating = players_clean["market_value_eur"] / (players_clean["overall"] + 1.0)
    value_per_rating_diff = (players_clean["value_per_rating"] - expected_value_per_rating).abs()
    value_per_rating_mismatch_rows = int((value_per_rating_diff > 1e-9).sum())
    value_per_rating_nonfinite_rows = int((~np.isfinite(players_clean["value_per_rating"])).sum())

    nonpositive_market_value_rows = int((players_clean["market_value_eur"] <= 0).sum())
    low_value_threshold_eur = 10_000.0
    low_value_rows_at_or_below_threshold = int(
        (players_clean["market_value_eur"] <= low_value_threshold_eur).sum()
    )

    one_hot_missing_columns = [
        col for col in POSITION_DUMMY_COLUMNS if col not in model_ready.columns
    ]

    one_hot_invalid_rows = -1
    if not one_hot_missing_columns:
        one_hot_sum = model_ready[POSITION_DUMMY_COLUMNS].sum(axis=1)
        one_hot_invalid_rows = int((one_hot_sum != 1).sum())

    range_checks = {
        "age_out_of_range_rows": int((~players_clean["age"].between(15, 45)).sum()),
        "overall_out_of_range_rows": int((~players_clean["overall"].between(40, 99)).sum()),
        "potential_out_of_range_rows": int((~players_clean["potential"].between(40, 99)).sum()),
    }

    for skill_col in ["pace", "shooting", "passing", "dribbling", "defending", "physic"]:
        range_checks[f"{skill_col}_out_of_range_rows"] = int(
            (~players_clean[skill_col].between(0, 99)).sum()
        )

    return {
        "age_potential_gap_mismatch_rows": age_gap_mismatch_rows,
        "age_potential_gap_zero_rows": age_gap_zero_rows,
        "is_at_peak_mismatch_rows": is_at_peak_mismatch_rows,
        "is_at_peak_share": is_at_peak_share,
        "value_per_rating_mismatch_rows": value_per_rating_mismatch_rows,
        "value_per_rating_nonfinite_rows": value_per_rating_nonfinite_rows,
        "nonpositive_market_value_rows": nonpositive_market_value_rows,
        "low_value_threshold_eur": low_value_threshold_eur,
        "low_value_rows_at_or_below_threshold": low_value_rows_at_or_below_threshold,
        "one_hot_missing_columns": one_hot_missing_columns,
        "one_hot_invalid_rows": one_hot_invalid_rows,
        "range_checks": range_checks,
    }


def build_report(players_clean: pd.DataFrame, model_ready: pd.DataFrame) -> Dict[str, object]:
    require_columns(players_clean, REQUIRED_PLAYERS_CLEAN_COLUMNS, "players_clean")
    require_columns(model_ready, ["log_value", "market_value_eur", "short_name"], "model_ready")

    position_counts = players_clean["position_group"].value_counts().sort_index()
    position_distribution = {
        group: {
            "count": int(count),
            "share": float(count / players_clean.shape[0]) if players_clean.shape[0] else 0.0,
        }
        for group, count in position_counts.items()
    }

    position_summary_df = (
        players_clean.groupby("position_group", as_index=True)
        .agg(
            count=("short_name", "count"),
            age_mean=("age", "mean"),
            overall_mean=("overall", "mean"),
            potential_mean=("potential", "mean"),
            market_value_median=("market_value_eur", "median"),
            market_value_mean=("market_value_eur", "mean"),
        )
        .sort_index()
    )
    position_summary = {
        group: {
            "count": int(row["count"]),
            "age_mean": to_float(row["age_mean"]),
            "overall_mean": to_float(row["overall_mean"]),
            "potential_mean": to_float(row["potential_mean"]),
            "market_value_median": to_float(row["market_value_median"]),
            "market_value_mean": to_float(row["market_value_mean"]),
        }
        for group, row in position_summary_df.to_dict(orient="index").items()
    }

    corr_source = model_ready.select_dtypes(include=[np.number])
    correlations_with_log_value: Dict[str, float] = {}
    if "log_value" in corr_source.columns:
        log_corr = corr_source.corr(numeric_only=True)["log_value"].drop(labels=["log_value"])
        log_corr = log_corr.dropna().sort_values(key=np.abs, ascending=False)
        correlations_with_log_value = {
            col: to_float(value) for col, value in log_corr.items()
        }

    outlier_summary = {
        "market_value_eur": iqr_outlier_summary(players_clean["market_value_eur"]),
        "log_value": iqr_outlier_summary(model_ready["log_value"]),
        "overall": iqr_outlier_summary(players_clean["overall"]),
        "potential": iqr_outlier_summary(players_clean["potential"]),
        "age": iqr_outlier_summary(players_clean["age"]),
    }

    return {
        "generated_at_utc": datetime.now(timezone.utc).isoformat(),
        "dataset_overview": {
            "players_clean_shape": {
                "rows": int(players_clean.shape[0]),
                "columns": int(players_clean.shape[1]),
            },
            "model_ready_shape": {
                "rows": int(model_ready.shape[0]),
                "columns": int(model_ready.shape[1]),
            },
        },
        "missingness": {
            "players_clean": summarize_missingness(players_clean),
            "model_ready": summarize_missingness(model_ready),
        },
        "numeric_summary": {
            "players_clean": summarize_numeric(players_clean),
            "model_ready": summarize_numeric(model_ready),
        },
        "position_group_distribution": position_distribution,
        "position_group_summary": position_summary,
        "outlier_summary_iqr": outlier_summary,
        "correlations_with_log_value": correlations_with_log_value,
        "feature_checks": compute_feature_checks(players_clean, model_ready),
    }


def report_to_markdown(report: Dict[str, object]) -> str:
    overview = report["dataset_overview"]
    missing_players = report["missingness"]["players_clean"]
    missing_model = report["missingness"]["model_ready"]
    feature_checks = report["feature_checks"]
    position_distribution = report["position_group_distribution"]
    correlations = report["correlations_with_log_value"]

    lines: List[str] = []
    lines.append("# FIFA EDA and Feature Validation Summary")
    lines.append("")
    lines.append(f"Generated at (UTC): {report['generated_at_utc']}")
    lines.append("")
    lines.append("## Dataset Overview")
    lines.append(
        f"- players_clean rows/cols: {overview['players_clean_shape']['rows']} / {overview['players_clean_shape']['columns']}"
    )
    lines.append(
        f"- model_ready rows/cols: {overview['model_ready_shape']['rows']} / {overview['model_ready_shape']['columns']}"
    )
    lines.append("")
    lines.append("## Missingness")
    lines.append(f"- players_clean total nulls: {missing_players['total_nulls']}")
    lines.append(f"- model_ready total nulls: {missing_model['total_nulls']}")
    lines.append("")
    lines.append("## Position Group Distribution")
    for group, values in position_distribution.items():
        lines.append(f"- {group}: {values['count']} ({values['share']:.2%})")
    lines.append("")
    lines.append("## Feature Checks")
    lines.append(
        f"- age_potential_gap mismatches: {feature_checks['age_potential_gap_mismatch_rows']}"
    )
    lines.append(
        f"- age_potential_gap == 0 rows: {feature_checks['age_potential_gap_zero_rows']}"
    )
    lines.append(
        f"- is_at_peak mismatches: {feature_checks['is_at_peak_mismatch_rows']}"
    )
    lines.append(
        f"- is_at_peak share: {feature_checks['is_at_peak_share']:.2%}"
    )
    lines.append(
        f"- value_per_rating mismatches: {feature_checks['value_per_rating_mismatch_rows']}"
    )
    lines.append(
        f"- value_per_rating non-finite rows: {feature_checks['value_per_rating_nonfinite_rows']}"
    )
    lines.append(
        f"- non-positive market_value_eur rows: {feature_checks['nonpositive_market_value_rows']}"
    )
    lines.append(
        "- low-value market_value_eur rows "
        f"(<= {feature_checks['low_value_threshold_eur']:.0f}): "
        f"{feature_checks['low_value_rows_at_or_below_threshold']}"
    )
    lines.append(f"- one-hot missing columns: {feature_checks['one_hot_missing_columns']}")
    lines.append(f"- one-hot invalid rows: {feature_checks['one_hot_invalid_rows']}")
    lines.append(
        "- position_group is one-hot encoded as: "
        f"{POSITION_DUMMY_COLUMNS}"
    )
    lines.append("")
    lines.append("## Strongest Correlations with log_value")
    top_items = list(correlations.items())[:10]
    for col, value in top_items:
        lines.append(f"- {col}: {value:.4f}")
    lines.append("")
    lines.append("## Outlier Snapshot (IQR Method)")
    for col, stats in report["outlier_summary_iqr"].items():
        lines.append(
            f"- {col}: outliers={stats['outlier_count']} ({stats['outlier_share']:.2%}), "
            f"bounds=[{stats['lower_bound']:.2f}, {stats['upper_bound']:.2f}]"
        )
    lines.append("")
    lines.append("For full details, see the paired JSON report.")

    return "\n".join(lines)


def run(paths: EDAPaths) -> None:
    if not paths.players_clean_path.exists():
        raise FileNotFoundError(
            f"players_clean file not found at {paths.players_clean_path}. Run preprocessing first."
        )
    if not paths.model_ready_path.exists():
        raise FileNotFoundError(
            f"model_ready file not found at {paths.model_ready_path}. Run preprocessing first."
        )

    players_clean = pd.read_csv(paths.players_clean_path)
    model_ready = pd.read_csv(paths.model_ready_path)

    report = build_report(players_clean, model_ready)

    paths.report_json_path.parent.mkdir(parents=True, exist_ok=True)
    paths.report_markdown_path.parent.mkdir(parents=True, exist_ok=True)

    with paths.report_json_path.open("w", encoding="utf-8") as f:
        json.dump(report, f, indent=2)

    markdown = report_to_markdown(report)
    paths.report_markdown_path.write_text(markdown, encoding="utf-8")

    print("EDA and feature validation completed successfully")
    print(f"JSON report: {paths.report_json_path}")
    print(f"Markdown summary: {paths.report_markdown_path}")


if __name__ == "__main__":
    repo_root = Path(__file__).resolve().parents[2]
    default_paths = EDAPaths(
        players_clean_path=repo_root / "ML" / "data" / "processed" / "players_clean.csv",
        model_ready_path=repo_root / "ML" / "data" / "features" / "model_ready.csv",
        report_json_path=repo_root / "ML" / "data" / "processed" / "eda_report.json",
        report_markdown_path=repo_root / "ML" / "data" / "processed" / "eda_summary.md",
    )
    run(default_paths)