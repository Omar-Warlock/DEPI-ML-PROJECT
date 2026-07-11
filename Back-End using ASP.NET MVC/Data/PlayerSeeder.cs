using CsvHelper;
using ScoutIQ.Models;
using System.Globalization;
using CsvHelper.Configuration;

namespace ScoutIQ.Data;

public static class PlayerSeeder
{
    public static List<Player> LoadPlayers(string filePath)
    {
        using var reader = new StreamReader(filePath);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            BadDataFound = null,
            HeaderValidated = null
        };

        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords<PlayerCsv>().ToList();

        Console.WriteLine($"Players Loaded = {records.Count}");

        return records.Select(x => new Player
        {
            player_id = x.player_id,

            name = x.name,

            club = x.club,

            country_of_birth = x.country_of_birth,
            country_of_citizenship = x.country_of_citizenship,

            sub_position = x.sub_position,
            position = x.position ?? -1,

            foot = x.foot,

            height_in_cm = x.height_in_cm,

            international_caps = x.international_caps,
            international_goals = x.international_goals,

            age = (int)x.age,

            log_market_value = x.log_market_value,

            contract_years_left = x.contract_years_left,

            total_games = x.total_games,
            total_goals = x.total_goals,
            total_assists = x.total_assists,

            total_yellow = x.total_yellow,
            total_red = x.total_red,

            total_minutes = x.total_minutes,
            avg_minutes = x.avg_minutes,

            goals_per_game = x.goals_per_game,
            assists_per_game = x.assists_per_game,

            num_transfers = x.num_transfers,

            total_transfer_fee = x.total_transfer_fee,
            max_transfer_fee = x.max_transfer_fee,
            avg_transfer_fee = x.avg_transfer_fee,

            goals_per_90 = x.goals_per_90,
            assists_per_90 = x.assists_per_90,

            goal_contributions = x.goal_contributions,
            gc_per_90 = x.gc_per_90,

            discipline_score = x.discipline_score,
            experience_score = x.experience_score,

            intl_ratio = x.intl_ratio,
            value_growth = x.value_growth,

            is_international = x.is_international,

            position_enc = x.position_enc,
            foot_enc = x.foot_enc,

            market_value_in_eur = x.market_value_in_eur,

            highest_market_value_in_eur = x.highest_market_value_in_eur
        }).ToList();
    }
}