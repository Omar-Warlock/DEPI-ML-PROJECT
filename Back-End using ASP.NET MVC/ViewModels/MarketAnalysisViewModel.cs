using ScoutIQ.Models;

namespace ScoutIQ.ViewModels;

public class MarketAnalysisViewModel
{
    public decimal TotalMarketValue { get; set; }

    public decimal AverageClubValue { get; set; }

    public int TotalPlayers { get; set; }

    public List<Player> TopValuablePlayers { get; set; } = new();

    public List<Player> HighGrowthPlayers { get; set; } = new();
    public List<Player> TopPlayers { get; set; } = new();
}