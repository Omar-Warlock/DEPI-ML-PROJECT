using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScoutIQ.Data;
using ScoutIQ.Models;
using ScoutIQ.Services;
using ScoutIQ.ViewModels;
using System.Text;


namespace ScoutIQ.Controllers;

public class PlayersController : Controller
{
    private readonly ScoutIQDbContext _context;
    private readonly PlayerCacheService _cache;

    public PlayersController(
      ScoutIQDbContext context,
      PlayerCacheService cache)
    {
        _context = context;
        _cache = cache;
    }


    [HttpGet]
    public IActionResult Index()
    {
        var vm = new PlayersViewModel
        {
            Players = new List<Player>(),

            VerifiedProfiles = 0,

            AverageMarketValue = 0,

            PredictedMarketValue = 0,

            BestPlayer = null,

            SelectedPosition = "ALL",

            SelectedMarketValue = "Any Value",

            SelectedMinAge = 16,

            SelectedMaxAge = 35
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> GeneratePredictions()
    {
        var players = _context.Players
     .OrderByDescending(x => x.market_value_in_eur)
     .Take(500)
     .ToList();
        Console.WriteLine($"Loaded {players.Count} players");

        using var client = new HttpClient();

        foreach (var player in players)
        {
            if (player.Predicted_Market_Value_EUR.HasValue)
                continue;

            var payload = new
            {
                country_of_birth = player.country_of_birth,
                country_of_citizenship = player.country_of_citizenship,
                sub_position = player.sub_position,
                position = player.position,

                height_in_cm = player.height_in_cm,
                international_caps = player.international_caps,
                international_goals = player.international_goals,

                age = player.age,
                contract_years_left = player.contract_years_left,

                total_games = player.total_games,
                total_assists = player.total_assists,
                total_red = player.total_red,

                avg_minutes = player.avg_minutes,

                goals_per_game = player.goals_per_game,
                assists_per_game = player.assists_per_game,

                num_transfers = player.num_transfers,

                max_transfer_fee = player.max_transfer_fee,
                avg_transfer_fee = player.avg_transfer_fee,

                goals_per_90 = player.goals_per_90,
                assists_per_90 = player.assists_per_90,

                gc_per_90 = player.gc_per_90,

                discipline_score = player.discipline_score,

                intl_ratio = player.intl_ratio,

                value_growth = player.value_growth,

                is_international = player.is_international,

                position_enc = player.position_enc,
                foot_enc = player.foot_enc,

                foot = player.foot
            };

            var json = JsonConvert.SerializeObject(payload);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "http://127.0.0.1:5000/predict",
                content);

            if (!response.IsSuccessStatusCode)
                continue;

            var result = await response.Content.ReadAsStringAsync();

            var prediction =
                JsonConvert.DeserializeObject<PredictionResponse>(result);

            if (prediction != null)
            {
                player.Predicted_Market_Value_EUR =
                    prediction.predicted_market_value;
            }
        }

        await _context.SaveChangesAsync();

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult ApplyFilters(
        int MinAge,
        int MaxAge,
        string Position,
        string MarketValue)
    {


        var query = _cache.Players.AsQueryable();


        Console.WriteLine($"Position Raw = [{Position}]");
        Console.WriteLine($"MarketValue = [{MarketValue}]");


        // Age Filter

        query = query.Where(p =>
            p.age >= MinAge &&
            p.age <= MaxAge);



        // Position Filter

        if (!string.IsNullOrEmpty(Position) &&
            Position != "ALL")
        {

            int positionValue = Position switch
            {
                "FWD" => 0,
                "DEF" => 1,
                "GK" => 2,
                "MID" => 3,
                _ => -1
            };


            Console.WriteLine($"Position Value = {positionValue}");


            if (positionValue != -1)
            {
                query = query.Where(p =>
                    p.position == positionValue);
            }

        }




        // Market Value Filter

        if (MarketValue == "15M+")
        {
            query = query.Where(p =>
                p.market_value_in_eur >= 15000000);
        }

        else if (MarketValue == "25M+")
        {
            query = query.Where(p =>
                p.market_value_in_eur >= 25000000);
        }

        else if (MarketValue == "35M+")
        {
            query = query.Where(p =>
                p.market_value_in_eur >= 35000000);
        }



        var players = query
            .OrderByDescending(p => p.market_value_in_eur)
            .Take(100)
            .ToList();



        Console.WriteLine($"Players Count = {players.Count}");



       
        var vm = new PlayersViewModel
        {

            Players = players,


            VerifiedProfiles = players.Count,


            AverageMarketValue = players.Any()
                ? players.Average(p => p.market_value_in_eur)
                : 0,


            PredictedMarketValue = players
    .Where(p => p.Predicted_Market_Value_EUR.HasValue)
    .Select(p => p.Predicted_Market_Value_EUR!.Value)
    .DefaultIfEmpty(0)
    .Average(),


            BestPlayer = players.FirstOrDefault(),



            // Keep Filters Selected

            SelectedPosition = Position,

            SelectedMarketValue = MarketValue,

            SelectedMinAge = MinAge,

            SelectedMaxAge = MaxAge

        };



        return View("Index", vm);

    }

}