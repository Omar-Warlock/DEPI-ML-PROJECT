using Microsoft.Extensions.DependencyInjection;
using ScoutIQ.Data;
using ScoutIQ.Models;

namespace ScoutIQ.Services;

public class PlayerCacheService
{
    public List<Player> Players { get; private set; } = new();

    public PlayerCacheService(IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ScoutIQDbContext>();

        LoadPlayers(context);
    }


    private void LoadPlayers(ScoutIQDbContext context)
    {
        Players = context.Players
            .ToList();

        Console.WriteLine($"Loaded Players = {Players.Count}");
    }
}