namespace ScoutIQ.Models;

public class Player
{
    public int Id { get; set; }

    public int player_id { get; set; }

    public string name { get; set; } = "";

    public string club { get; set; } = "";

    public string country_of_birth { get; set; } = "";

    public string country_of_citizenship { get; set; } = "";

    public string sub_position { get; set; } = "";

    public int position { get; set; }
    public string PositionName
    {
        get
        {
            return position switch
            {
                0 => "FWD",
                1 => "DEF",
                2 => "GK",
                3 => "MID",
                _ => "Unknown"
            };
        }
    }

    public string foot { get; set; } = "";


    public double height_in_cm { get; set; }

    public double international_caps { get; set; }

    public double international_goals { get; set; }


    // FIX
    public int age { get; set; }


    public double log_market_value { get; set; }

    public double contract_years_left { get; set; }


    public double total_games { get; set; }

    public double total_goals { get; set; }

    public double total_assists { get; set; }

    public double total_yellow { get; set; }

    public double total_red { get; set; }

    public double total_minutes { get; set; }

    public double avg_minutes { get; set; }


    public double goals_per_game { get; set; }

    public double assists_per_game { get; set; }


    public double num_transfers { get; set; }

    public double total_transfer_fee { get; set; }

    public double max_transfer_fee { get; set; }

    public double avg_transfer_fee { get; set; }


    public double goals_per_90 { get; set; }

    public double assists_per_90 { get; set; }


    public double goal_contributions { get; set; }

    public double gc_per_90 { get; set; }


    public double discipline_score { get; set; }

    public double experience_score { get; set; }


    public double intl_ratio { get; set; }

    public double value_growth { get; set; }


    public int is_international { get; set; }

    public int position_enc { get; set; }

    public int foot_enc { get; set; }


    public decimal market_value_in_eur { get; set; }

    public decimal highest_market_value_in_eur { get; set; }


    // ML Prediction
    public decimal? Predicted_Market_Value_EUR { get; set; }
    public double Rating
    {
        get
        {
            double score = 0;

            score += Math.Min(goals_per_game * 20, 3);

            score += Math.Min(assists_per_game * 20, 2);

            score += Math.Min(experience_score / 20, 2);

            score += Math.Min(intl_ratio * 2, 1.5);

            score += Math.Min(value_growth / 50, 1.5);

            return Math.Round(score, 1);
        }
    }
    public decimal Potential_Gain
    {
        get
        {
            if (Predicted_Market_Value_EUR == null)
                return 0;

            return Predicted_Market_Value_EUR.Value - market_value_in_eur;
        }
    }
    public decimal MarketDelta
    {
        get
        {
            return highest_market_value_in_eur - market_value_in_eur;
        }
    }
    public double UndervaluedScore
    {
        get
        {
            if (market_value_in_eur == 0 || Predicted_Market_Value_EUR == null)
                return 0;


            return (double)(
                ((Predicted_Market_Value_EUR.Value - market_value_in_eur)
                / market_value_in_eur)
                * 100
            );
        }
    }
}

