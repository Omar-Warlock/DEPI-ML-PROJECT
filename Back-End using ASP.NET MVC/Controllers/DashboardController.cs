using Microsoft.AspNetCore.Mvc;
using ScoutIQ.Services;
using ScoutIQ.ViewModels;

namespace ScoutIQ.Controllers;

public class DashboardController : Controller
{
    private readonly PlayerCacheService _cache;

    public DashboardController(PlayerCacheService cache)
    {
        _cache = cache;
    }


    public IActionResult Index()
    {
        var players = _cache.Players ?? new List<ScoutIQ.Models.Player>();


        // ============================
        // Market Average By Position
        // ============================

        decimal forwardAverage = players
     .Where(x => x.PositionName == "FWD")
     .Select(x => x.market_value_in_eur)
     .DefaultIfEmpty(0)
     .Average();


        decimal midfieldAverage = players
            .Where(x => x.PositionName == "MID")
            .Select(x => x.market_value_in_eur)
           .DefaultIfEmpty(0)
            .Average();


        decimal defenseAverage = players
            .Where(x => x.PositionName == "DEF")
            .Select(x => x.market_value_in_eur)
            .DefaultIfEmpty(0)
            .Average();


        decimal goalkeeperAverage = players
            .Where(x => x.PositionName == "GK")
            .Select(x => x.market_value_in_eur)
           .DefaultIfEmpty(0)
            .Average();



        // ============================
        // Normalize Chart Percentages
        // ============================

        decimal maxValue = new[]
        {
            forwardAverage,
            midfieldAverage,
            defenseAverage,
            goalkeeperAverage
        }
        .Max();



        double forwardPercentage = maxValue > 0
            ? (double)(forwardAverage / maxValue * 100)
            : 0;


        double midfieldPercentage = maxValue > 0
            ? (double)(midfieldAverage / maxValue * 100)
            : 0;


        double defensePercentage = maxValue > 0
            ? (double)(defenseAverage / maxValue * 100)
            : 0;


        double goalkeeperPercentage = maxValue > 0
            ? (double)(goalkeeperAverage / maxValue * 100)
            : 0;




        // ============================
        // Undervalued Players
        // ============================

        var undervaluedPlayers = players
    .Where(x =>
        x.Predicted_Market_Value_EUR.HasValue &&
        x.market_value_in_eur > 0)
  .Select(x =>
  {
      var growth =
          ((x.Predicted_Market_Value_EUR!.Value
          - x.market_value_in_eur)
          / x.market_value_in_eur) * 100;


      x.value_growth = (double)growth;

      return x;
  })
    .Where(x => x.value_growth > 10)
    .OrderByDescending(x => x.value_growth)
    .Take(5)
    .ToList();





        var vm = new DashboardViewModel
        {
            Players = players,


            VerifiedProfiles = players.Count,


            AverageMarketValue = players.Any()
                ? players.Average(x => x.market_value_in_eur)
                : 0,

            PredictedMarketValue = players
    .Where(x => x.Predicted_Market_Value_EUR.HasValue)
    .Select(x => x.Predicted_Market_Value_EUR!.Value)
    .DefaultIfEmpty(0)
    .Average(),


            BestPlayer = players
                .OrderByDescending(x => x.Rating)
                .FirstOrDefault(),



            // Undervalued
            UndervaluedPlayers = undervaluedPlayers,

        };





        Console.WriteLine(
    $"Players With Prediction = {players.Count(x => x.Predicted_Market_Value_EUR.HasValue)}"
);

        return View(vm);

    }
}