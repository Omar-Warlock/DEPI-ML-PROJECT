from flask import Flask, request, jsonify
import joblib
import pandas as pd

app = Flask(__name__)

model = joblib.load("scout_model.joblib")
scaler = joblib.load("scaler.joblib")

@app.route("/predict", methods=["POST"])
def predict():
    data = request.json

    features = [[
        data["age"],
        data["overall"],
        data["potential"],
        data["age_potential_gap"],
        data["is_at_peak"],
        data["pace"],
        data["shooting"],
        data["passing"],
        data["dribbling"],
        data["defending"],
        data["physic"],
        data["position_group_GK"],
        data["position_group_DEF"],
        data["position_group_MID"],
        data["position_group_ATT"]
    ]]

    X = pd.DataFrame(features)
    X_scaled = scaler.transform(X)
    prediction = model.predict(X_scaled)[0]

    return jsonify({
        "predicted_market_value": float(prediction)
    })

if __name__ == "__main__":
    app.run(debug=True)