using Microsoft.EntityFrameworkCore;
using ScoutIQ.Models;

namespace ScoutIQ.Data;

public class ScoutIQDbContext : DbContext
{
    public ScoutIQDbContext(
        DbContextOptions<ScoutIQDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Player>()
            .Property(p => p.market_value_in_eur)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Player>()
            .Property(p => p.highest_market_value_in_eur)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Player>()
            .Property(p => p.Predicted_Market_Value_EUR)
            .HasPrecision(18, 2);
    
}
}