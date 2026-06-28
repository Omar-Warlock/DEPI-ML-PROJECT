namespace ScoutIQ.ViewModels;

public class PlayerPredictionViewModel
{
    public int Age { get; set; }

    public decimal MarketValue { get; set; }

    public string Position { get; set; } = string.Empty;

    public double ScoutRating { get; set; }

    public string Potential { get; set; } = string.Empty;
}