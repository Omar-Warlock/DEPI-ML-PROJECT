using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScoutIQ.Services;
using ScoutIQ.ViewModels;

namespace ScoutIQ.Controllers;

public class MarketAnalysisController : Controller
{
    private readonly PlayerCacheService _cache;

    public MarketAnalysisController(PlayerCacheService cache)
    {
        _cache = cache;
    }

    public IActionResult Index()
    {
        var players = _cache.Players;

        var vm = new MarketAnalysisViewModel
        {
            TotalPlayers = players.Count,

            TotalMarketValue = players.Sum(x => x.market_value_in_eur),

            AverageClubValue = players.Any()
                ? players.Average(x => x.market_value_in_eur)
                : 0,

            TopValuablePlayers = players
                .OrderByDescending(x => x.market_value_in_eur)
                .Take(5)
                .ToList(),

            HighGrowthPlayers = players
                .OrderByDescending(x => x.value_growth)
                .Take(4)
                .ToList(),

            TopPlayers = players
                .OrderByDescending(x => x.value_growth)
                .Take(4)
                .ToList()
        };

        return View(vm);
    }
}