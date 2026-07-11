using ScoutIQ.Models;

namespace ScoutIQ.ViewModels;

public class PlayersViewModel
{
    public List<Player> Players { get; set; } = new();

    public decimal AverageMarketValue { get; set; }

    public int VerifiedProfiles { get; set; }

    public decimal PredictedMarketValue { get; set; }
    public Player? BestPlayer { get; set; }


    public string? SelectedPosition { get; set; }

    public string? SelectedMarketValue { get; set; }

    public int SelectedMinAge { get; set; }

    public int SelectedMaxAge { get; set; }
}