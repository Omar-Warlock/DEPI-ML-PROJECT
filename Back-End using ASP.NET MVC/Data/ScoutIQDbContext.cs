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
        modelBuilder.Entity<Player>()
      .Property(p => p.market_value_eur)
      .HasPrecision(18, 2);
    }
}