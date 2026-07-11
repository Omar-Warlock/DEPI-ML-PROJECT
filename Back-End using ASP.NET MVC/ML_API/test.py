import requests

url = "http://127.0.0.1:5000/predict"

data = {
    "country_of_birth": "Egypt",
    "country_of_citizenship": "Egypt",
    "sub_position": "Centre-Forward",
    "position": "Attack",

    "height_in_cm": 185,
    "international_caps": 20,
    "international_goals": 5,

    "age": 22,
    "contract_years_left": 3,

    "total_games": 100,
    "total_assists": 20,
    "total_red": 1,

    "avg_minutes": 75,

    "goals_per_game": 0.5,
    "assists_per_game": 0.2,

    "num_transfers": 2,

    "max_transfer_fee": 5000000,
    "avg_transfer_fee": 2000000,

    "goals_per_90": 0.6,
    "assists_per_90": 0.3,

    "gc_per_90": 0.9,
    "discipline_score": 80,

    "intl_ratio": 0.5,
    "value_growth": 0.2,

    "is_international": 1,

    "position_enc": 0,
    "foot": "Right",
}


response = requests.post(
    url,
    json=data
)

print("Status Code:", response.status_code)
print("Response:")
print(response.text)