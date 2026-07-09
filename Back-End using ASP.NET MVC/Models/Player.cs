namespace ScoutIQ.Models;

public class Player
{
    public int Id { get; set; }

    public string short_name { get; set; } = default!;

    public int age { get; set; }

    public double overall { get; set; }

    public double potential { get; set; }

    public double age_potential_gap { get; set; }

    public double pace { get; set; }

    public double shooting { get; set; }

    public double passing { get; set; }

    public double dribbling { get; set; }

    public double defending { get; set; }

    public double physic { get; set; }

    public int position_group_GK { get; set; }

    public int position_group_MID { get; set; }

    public int position_group_ATT { get; set; }

    public decimal market_value_eur { get; set; }

    public double value_per_rating { get; set; }

    public int is_at_peak { get; set; }

    public int position_group_DEF { get; set; }
}