import requests

response = requests.post(
    "http://127.0.0.1:5000/predict",
    json={
        "age": 22,
        "overall": 80,
        "potential": 88,
        "age_potential_gap": 8,
        "is_at_peak": 0,
        "pace": 75,
        "shooting": 70,
        "passing": 78,
        "dribbling": 80,
        "defending": 60,
        "physic": 72,
        "position_group_GK": 0,
        "position_group_DEF": 0,
        "position_group_MID": 1,
        "position_group_ATT": 0
    }
)

print(response.status_code)
print(response.json())