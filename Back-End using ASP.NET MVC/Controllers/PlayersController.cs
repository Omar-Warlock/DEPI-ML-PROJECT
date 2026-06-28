using Microsoft.AspNetCore.Mvc;
using ScoutIQ.Data;

namespace ScoutIQ.Controllers;

public class PlayersController : Controller
{
    private readonly ScoutIQDbContext _context;

    public PlayersController(ScoutIQDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var players = _context.Players.ToList();

        return View(players);
    }
}