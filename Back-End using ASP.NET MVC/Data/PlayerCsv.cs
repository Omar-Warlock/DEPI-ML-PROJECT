namespace ScoutIQ.Data;

public class PlayerCsv
{
    public string country_of_birth { get; set; } = "";
    public string country_of_citizenship { get; set; } = "";

    public string sub_position { get; set; } = "";
    public int? position { get; set; }
    public string foot { get; set; } = "";

    public double height_in_cm { get; set; }

    public double international_caps { get; set; }
    public double international_goals { get; set; }

    public double age { get; set; }

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

    public int player_id { get; set; }

    public string name { get; set; } = "";
    public string club { get; set; } = "";

    public decimal market_value_in_eur { get; set; }
    public decimal highest_market_value_in_eur { get; set; }
}