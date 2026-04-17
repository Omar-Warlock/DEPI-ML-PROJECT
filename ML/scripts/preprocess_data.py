from __future__ import annotations

import json
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, List, Tuple

import numpy as np
import pandas as pd


@dataclass
class PipelinePaths:
    raw_dir: Path
    processed_dir: Path
    features_dir: Path


MODEL_FEATURE_COLUMNS = [
    "age",
    "overall",
    "potential",
    "age_potential_gap",
    "is_at_peak",
    "pace",
    "shooting",
    "passing",
    "dribbling",
    "defending",
    "physic",
    "position_group_GK",
    "position_group_DEF",
    "position_group_MID",
    "position_group_ATT",
]

MODEL_READY_COLUMNS = MODEL_FEATURE_COLUMNS + [
    "log_value",
    "short_name",
    "market_value_eur",
    "value_per_rating",
]

# Conservative quantile winsorization for valid but extreme values.
OUTLIER_WINSOR_CONFIG: Dict[str, Dict[str, float]] = {
    "market_value_eur": {"lower": 0.01, "upper": 0.99},
}

RAW_REQUIRED_COLUMNS = [
    "player_id",
    "fifa_version",
    "fifa_update",
    "short_name",
    "age",
    "overall",
    "potential",
    "value_eur",
    "player_positions",
    "pace",
    "shooting",
    "passing",
    "dribbling",
    "defending",
    "physic",
]


def normalize_column(col_name: str) -> str:
    return col_name.strip().lower().replace(" ", "_")


def parse_market_value(value: object) -> float:
    if pd.isna(value):
        return np.nan
    if isinstance(value, (int, float, np.integer, np.floating)):
        return float(value)

    text = str(value).strip().upper()
    if not text:
        return np.nan

    text = (
        text.replace("€", "")
        .replace("EUR", "")
        .replace(",", "")
        .replace(" ", "")
    )

    multiplier = 1.0
    if text.endswith("M"):
        multiplier = 1_000_000.0
        text = text[:-1]
    elif text.endswith("K"):
        multiplier = 1_000.0
        text = text[:-1]
    elif text.endswith("B"):
        multiplier = 1_000_000_000.0
        text = text[:-1]

    try:
        return float(text) * multiplier
    except ValueError:
        return np.nan


def infer_fifa_file(raw_dir: Path) -> Path:
    csv_files = sorted(raw_dir.rglob("*.csv"))
    if not csv_files:
        raise FileNotFoundError(
            "No CSV files found in ML/data/raw. Add FIFA 23 CSV first."
        )

    required_like = {
        "age",
        "overall",
        "potential",
        "value_eur",
        "player_positions",
    }

    best_match: Tuple[int, Path] | None = None
    for csv_path in csv_files:
        try:
            header = pd.read_csv(csv_path, nrows=0)
        except Exception:
            continue

        normalized_cols = {normalize_column(c) for c in header.columns}
        score = len(required_like.intersection(normalized_cols))

        # Prefer likely FIFA files if tied.
        name_boost = 1 if "fifa" in csv_path.name.lower() else 0
        score += name_boost

        if best_match is None or score > best_match[0]:
            best_match = (score, csv_path)

    if best_match is None or best_match[0] < 4:
        raise FileNotFoundError(
            "Could not detect FIFA 23 CSV in ML/data/raw. "
            "Expected columns like age, overall, potential, value_eur, player_positions."
        )

    return best_match[1]


def load_fifa_dataframe(fifa_file: Path) -> pd.DataFrame:
    required_set = set(RAW_REQUIRED_COLUMNS)

    # Use selective column loading to avoid OOM on very large source CSVs.
    df = pd.read_csv(
        fifa_file,
        usecols=lambda col_name: normalize_column(col_name) in required_set,
        low_memory=True,
    )
    df.columns = [normalize_column(c) for c in df.columns]

    # If the file contains many FIFA versions, keep only FIFA 23 records.
    if "fifa_version" in df.columns:
        df = df[df["fifa_version"] == 23].copy()

    # If there are multiple updates per player, keep the latest snapshot.
    if all(col in df.columns for col in ["player_id", "fifa_update"]):
        df["fifa_update"] = pd.to_numeric(df["fifa_update"], errors="coerce")
        df = df.sort_values(["player_id", "fifa_update"]).drop_duplicates(
            subset=["player_id"], keep="last"
        )

    return df.reset_index(drop=True)


def enforce_required_columns(df: pd.DataFrame, required_columns: List[str]) -> None:
    missing = [col for col in required_columns if col not in df.columns]
    if missing:
        raise ValueError(f"Missing required columns after cleaning: {missing}")


def build_position_group(player_positions: pd.Series) -> pd.Series:
    primary = (
        player_positions.fillna("")
        .astype(str)
        .str.split(",")
        .str[0]
        .str.strip()
        .str.upper()
    )

    mapping: Dict[str, str] = {
        "GK": "GK",
        "CB": "DEF",
        "LB": "DEF",
        "RB": "DEF",
        "LWB": "DEF",
        "RWB": "DEF",
        "CDM": "MID",
        "CM": "MID",
        "CAM": "MID",
        "LM": "MID",
        "RM": "MID",
        "LW": "ATT",
        "RW": "ATT",
        "ST": "ATT",
        "CF": "ATT",
        "LF": "ATT",
        "RF": "ATT",
    }
    return primary.map(mapping)


def winsorize_columns(
    df: pd.DataFrame,
    config: Dict[str, Dict[str, float]],
) -> Tuple[pd.DataFrame, Dict[str, Dict[str, float]]]:
    df = df.copy()
    summary: Dict[str, Dict[str, float]] = {}

    for col, bounds in config.items():
        if col not in df.columns:
            continue

        lower_q = float(bounds["lower"])
        upper_q = float(bounds["upper"])
        if not (0.0 <= lower_q < upper_q <= 1.0):
            raise ValueError(
                f"Invalid winsorization quantiles for '{col}': "
                f"lower={lower_q}, upper={upper_q}"
            )

        series = pd.to_numeric(df[col], errors="coerce")
        valid = series.dropna()
        if valid.empty:
            continue

        lower_bound = float(valid.quantile(lower_q))
        upper_bound = float(valid.quantile(upper_q))
        lower_clipped_rows = int((series < lower_bound).sum())
        upper_clipped_rows = int((series > upper_bound).sum())

        df[col] = series.clip(lower=lower_bound, upper=upper_bound)
        summary[col] = {
            "lower_quantile": lower_q,
            "upper_quantile": upper_q,
            "lower_bound": lower_bound,
            "upper_bound": upper_bound,
            "lower_clipped_rows": lower_clipped_rows,
            "upper_clipped_rows": upper_clipped_rows,
            "total_clipped_rows": lower_clipped_rows + upper_clipped_rows,
        }

    return df, summary


def preprocess_fifa_dataframe(df: pd.DataFrame) -> Tuple[pd.DataFrame, pd.DataFrame, dict]:
    df = df.copy()

    original_shape = df.shape

    # Step 2: Drop columns with >40% missingness.
    missing_ratio = df.isna().mean()
    high_missing_cols = missing_ratio[missing_ratio > 0.40].index.tolist()
    if high_missing_cols:
        df = df.drop(columns=high_missing_cols)

    # Step 3: Impute remaining numeric nulls.
    numeric_cols = df.select_dtypes(include=[np.number]).columns
    for col in numeric_cols:
        median_value = df[col].median()
        df[col] = df[col].fillna(median_value)

    required_base_columns = [
        "short_name",
        "age",
        "overall",
        "potential",
        "value_eur",
        "player_positions",
        "pace",
        "shooting",
        "passing",
        "dribbling",
        "defending",
        "physic",
    ]
    enforce_required_columns(df, required_base_columns)

    # Step 4: Parse market value and filter unusable rows.
    pre_market_rows = int(df.shape[0])
    df["market_value_eur"] = df["value_eur"].apply(parse_market_value)
    missing_market_value_rows = int(df["market_value_eur"].isna().sum())
    nonpositive_market_value_rows = int((df["market_value_eur"] <= 0).sum())
    df = df[df["market_value_eur"].notna()].copy()
    df = df[df["market_value_eur"] > 0].copy()
    removed_market_value_rows = pre_market_rows - int(df.shape[0])

    # Additional quality filters from the data contract ranges.
    df = df[df["age"].between(15, 45)]
    df = df[df["overall"].between(40, 99)]
    df = df[df["potential"].between(40, 99)]
    for skill_col in ["pace", "shooting", "passing", "dribbling", "defending", "physic"]:
        df = df[df[skill_col].between(0, 99)]

    # Step 5: Feature engineering.
    df["age_potential_gap"] = df["potential"] - df["overall"]
    # Peak indicator distinguishes players who already reached potential.
    df["is_at_peak"] = (df["age_potential_gap"] == 0).astype(int)
    df["value_per_rating"] = df["market_value_eur"] / (df["overall"] + 1.0)
    df["position_group"] = build_position_group(df["player_positions"])
    df = df[df["position_group"].notna()].copy()

    # Step 6: Winsorize selected skewed columns to limit extreme leverage.
    df, winsor_summary = winsorize_columns(df, OUTLIER_WINSOR_CONFIG)
    # Keep derived feature definition consistent after target winsorization.
    df["value_per_rating"] = df["market_value_eur"] / (df["overall"] + 1.0)

    dummies = pd.get_dummies(df["position_group"], prefix="position_group", dtype=int)
    for required_dummy in [
        "position_group_GK",
        "position_group_DEF",
        "position_group_MID",
        "position_group_ATT",
    ]:
        if required_dummy not in dummies.columns:
            dummies[required_dummy] = 0

    df = pd.concat([df, dummies], axis=1)

    df["log_value"] = np.log1p(df["market_value_eur"])

    # Final selection and validation.
    model_ready = df[MODEL_READY_COLUMNS].copy()
    if model_ready.isna().sum().sum() != 0:
        nulls = model_ready.isna().sum()
        non_zero_nulls = nulls[nulls > 0].to_dict()
        raise ValueError(f"Model-ready dataset has nulls: {non_zero_nulls}")

    # Deterministic row order for reproducibility.
    model_ready = model_ready.sort_values(["short_name", "market_value_eur"]).reset_index(drop=True)

    clean_columns = [
        "short_name",
        "age",
        "overall",
        "potential",
        "player_positions",
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
    players_clean = df[clean_columns].copy()
    players_clean = players_clean.sort_values(["short_name", "market_value_eur"]).reset_index(drop=True)

    report = {
        "original_shape": {"rows": int(original_shape[0]), "columns": int(original_shape[1])},
        "post_clean_shape": {"rows": int(players_clean.shape[0]), "columns": int(players_clean.shape[1])},
        "model_ready_shape": {"rows": int(model_ready.shape[0]), "columns": int(model_ready.shape[1])},
        "dropped_high_missing_columns": high_missing_cols,
        "market_value_quality": {
            "missing_market_value_rows": missing_market_value_rows,
            "nonpositive_market_value_rows": nonpositive_market_value_rows,
            "removed_market_value_rows": removed_market_value_rows,
        },
        "is_at_peak": {
            "count": int(df["is_at_peak"].sum()),
            "share": float(df["is_at_peak"].mean()) if len(df) else 0.0,
        },
        "winsorization": winsor_summary,
        "nulls_in_model_ready": int(model_ready.isna().sum().sum()),
    }

    return players_clean, model_ready, report


def run(paths: PipelinePaths) -> None:
    paths.processed_dir.mkdir(parents=True, exist_ok=True)
    paths.features_dir.mkdir(parents=True, exist_ok=True)

    fifa_file = infer_fifa_file(paths.raw_dir)
    raw_df = load_fifa_dataframe(fifa_file)

    players_clean, model_ready, report = preprocess_fifa_dataframe(raw_df)

    clean_path = paths.processed_dir / "players_clean.csv"
    model_ready_path = paths.features_dir / "model_ready.csv"
    report_path = paths.processed_dir / "preprocessing_report.json"

    players_clean.to_csv(clean_path, index=False)
    model_ready.to_csv(model_ready_path, index=False)
    report["fifa_file"] = str(fifa_file)
    report["players_clean_path"] = str(clean_path)
    report["model_ready_path"] = str(model_ready_path)

    with report_path.open("w", encoding="utf-8") as f:
        json.dump(report, f, indent=2)

    print("Preprocessing completed successfully")
    print(f"FIFA source: {fifa_file}")
    print(f"players_clean.csv shape: {players_clean.shape}")
    print(f"model_ready.csv shape: {model_ready.shape}")
    print(f"Report written to: {report_path}")


if __name__ == "__main__":
    repo_root = Path(__file__).resolve().parents[2]
    default_paths = PipelinePaths(
        raw_dir=repo_root / "ML" / "data" / "raw",
        processed_dir=repo_root / "ML" / "data" / "processed",
        features_dir=repo_root / "ML" / "data" / "features",
    )
    run(default_paths)