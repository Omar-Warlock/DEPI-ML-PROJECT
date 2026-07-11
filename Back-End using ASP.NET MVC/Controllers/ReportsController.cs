using Microsoft.AspNetCore.Mvc;
using ScoutIQ.Services;
using ScoutIQ.ViewModels;

namespace ScoutIQ.Controllers;

public class ReportsController : Controller
{
    private readonly PlayerCacheService _cache;

    public ReportsController(PlayerCacheService cache)
    {
        _cache = cache;
    }

    public IActionResult Index()
    {
        var vm = new ReportsViewModel
        {
            TopRatedPlayers = _cache.Players
                .OrderByDescending(x => x.Rating)
                .Take(6)
                .ToList()
        };

        return View(vm);
    }
}