using Microsoft.AspNetCore.Mvc;
using ScoutIQ.Data;
using ScoutIQ.Models;
using ScoutIQ.ViewModels;
using System.Text;
using Newtonsoft.Json;

namespace ScoutIQ.Controllers;

public class DashboardController : Controller
{
    private readonly ScoutIQDbContext _context;

    public DashboardController(ScoutIQDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var vm = new DashboardViewModel();

        return View(vm);
    }
    //post

    [HttpPost]
    public IActionResult ApplyFilters(
        int MinAge,
        int MaxAge,
        string Position,
        string MarketValue)
    {
        var query = _context.Players.AsQueryable();

        // Age Filter
        query = query.Where(p =>
            p.age >= MinAge &&
            p.age <= MaxAge);

        // Position Filter
        if (Position == "GK")
        {
            query = query.Where(p => p.position_group_GK == 1);
        }
        else if (Position == "DEF")
        {
            query = query.Where(p => p.position_group_DEF == 1);
        }
        else if (Position == "MID")
        {
            query = query.Where(p => p.position_group_MID == 1);
        }
        else if (Position == "FWD")
        {
            query = query.Where(p => p.position_group_ATT == 1);
        }

        // Market Value Filter
        if (MarketValue == "10M+")
        {
            query = query.Where(p => p.market_value_eur >= 10000000);
        }
        else if (MarketValue == "50M+")
        {
            query = query.Where(p => p.market_value_eur >= 50000000);
        }
        else if (MarketValue == "100M+")
        {
            query = query.Where(p => p.market_value_eur >= 100000000);
        }

        //ApplyFilters

        var players = query
    .OrderByDescending(p => p.overall)
    .Take(100)
    .ToList();
        decimal predictedValue = 0;

        if (players.Any())
        {
            var player = players.First();

            var payload = new
            {
                age = player.age,
                overall = player.overall,
                potential = player.potential,
                age_potential_gap = player.age_potential_gap,
                is_at_peak = player.is_at_peak,
                pace = player.pace,
                shooting = player.shooting,
                passing = player.passing,
                dribbling = player.dribbling,
                defending = player.defending,
                physic = player.physic,
                position_group_GK = player.position_group_GK,
                position_group_DEF = player.position_group_DEF,
                position_group_MID = player.position_group_MID,
                position_group_ATT = player.position_group_ATT
            };

            using var client = new HttpClient();

            var json = JsonConvert.SerializeObject(payload);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            //flask

            var response = client.PostAsync(
                "http://127.0.0.1:5000/predict",
                content).Result;

            var result = response.Content
                .ReadAsStringAsync()
                .Result;

            dynamic obj = JsonConvert.DeserializeObject(result);

            predictedValue =
     Convert.ToDecimal(obj.predicted_market_value);
        }

        var vm = new DashboardViewModel
        {
            Players = players,
            VerifiedProfiles = players.Count,

            AverageMarketValue = players.Any()
                ? players.Average(p => p.market_value_eur)
                : 0,

            PredictedMarketValue = predictedValue,

              BestPlayer = players.FirstOrDefault()

            // PredictedMarketValue = 0 // مؤقتًا
        };

        return View("Index", vm);
    }
}