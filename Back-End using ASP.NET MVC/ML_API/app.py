from flask import Flask, request, jsonify
import pandas as pd
import numpy as np
import joblib

from mappings import (
    country_birth_mapping,
    country_citizenship_mapping,
    sub_position_mapping,
    position_mapping,
    foot_mapping
)

app = Flask(__name__)

model = joblib.load("best_model.joblib")
scaler = joblib.load("scaler.joblib")
imputer = joblib.load("imputer.joblib")



@app.route("/predict", methods=["POST"])
def predict():

    try:

        data = request.json

        features = pd.DataFrame([{

            "country_of_birth":
                country_birth_mapping.get(data["country_of_birth"], 0),

            "country_of_citizenship":
                country_citizenship_mapping.get(data["country_of_citizenship"], 0),

            "sub_position":
                sub_position_mapping.get(data["sub_position"], 0),

            "position":
                position_mapping.get(data["position"], 0),

            "height_in_cm": data["height_in_cm"],
            "international_caps": data["international_caps"],
            "international_goals": data["international_goals"],
            "age": data["age"],
            "contract_years_left": data["contract_years_left"],
            "total_games": data["total_games"],
            "total_assists": data["total_assists"],
            "total_red": data["total_red"],
            "avg_minutes": data["avg_minutes"],
            "goals_per_game": data["goals_per_game"],
            "assists_per_game": data["assists_per_game"],
            "num_transfers": data["num_transfers"],
            "max_transfer_fee": data["max_transfer_fee"],
            "avg_transfer_fee": data["avg_transfer_fee"],
            "goals_per_90": data["goals_per_90"],
            "assists_per_90": data["assists_per_90"],
            "gc_per_90": data["gc_per_90"],
            "discipline_score": data["discipline_score"],
            "intl_ratio": data["intl_ratio"],
            "value_growth": data["value_growth"],
            "is_international": data["is_international"],
            "position_enc": data["position_enc"],

            "foot_enc":
                foot_mapping.get(data["foot"], 0)

        }])

        print(features)

        features = features[model.feature_names_in_]

        X = imputer.transform(features)

        X = scaler.transform(X)

        prediction = model.predict(X)[0]

        market_value = np.expm1(prediction)
        print("Prediction =", market_value)

        return jsonify({
            "predicted_market_value": float(market_value)
        })

    except Exception as e:
        return jsonify({
            "error": str(e)
            }), 500
   


    
if __name__ == "__main__":
    app.run(debug=True)