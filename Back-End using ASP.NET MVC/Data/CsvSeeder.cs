using CsvHelper;
using ScoutIQ.Models;
using System.Globalization;

namespace ScoutIQ.Data;

public static class CsvSeeder
{
    public static List<Player> LoadPlayers(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
       
        var records = csv.GetRecords<PlayerCsv>().ToList();
        Console.WriteLine(records.First().is_at_peak);
        Console.WriteLine(records.First().position_group_DEF);

        return records.Select(x => new Player
        {
            short_name = x.short_name,
            age = x.age,
            overall = x.overall,
            potential = x.potential,
            age_potential_gap = x.age_potential_gap,
            pace = x.pace,
            shooting = x.shooting,
            passing = x.passing,
            dribbling = x.dribbling,
            defending = x.defending,
            physic = x.physic,
            position_group_GK = x.position_group_GK,
            position_group_MID = x.position_group_MID,
            position_group_ATT = x.position_group_ATT,
            market_value_eur = x.market_value_eur,
            value_per_rating = x.value_per_rating,
            is_at_peak = x.is_at_peak,
            position_group_DEF = x.position_group_DEF,
        }).ToList();
    }
}

public class PlayerCsv
{
    public string short_name { get; set; } = default!;
    public int age { get; set; }
    public double overall { get; set; }
    public double potential { get; set; }
    public double age_potential_gap { get; set; }
    public double pace { get; set; }
    public double shooting { get; set; }
    public double passing { get; set; }
    public double dribbling { get; set; }
    public double defending { get; set; }
    public double physic { get; set; }

    public int position_group_GK { get; set; }
    public int position_group_MID { get; set; }
    public int position_group_ATT { get; set; }

    public decimal market_value_eur { get; set; }
    public double value_per_rating { get; set; }
    public int is_at_peak { get; set; }

    public int position_group_DEF { get; set; }
}