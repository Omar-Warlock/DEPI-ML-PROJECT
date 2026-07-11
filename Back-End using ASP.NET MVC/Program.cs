using Microsoft.EntityFrameworkCore;
using ScoutIQ.Data;
using ScoutIQ.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ScoutIQDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")
));


builder.Services.AddHttpClient();
builder.Services.AddSingleton<PlayerCacheService>();

var app = builder.Build();

// Seed CSV Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
    .GetRequiredService<ScoutIQDbContext>();


  

    if (!context.Players.Any())
    {
        var playersPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "DataFiles",
            "transfermarkt_cleaned_final11.csv"
        );

        var players = PlayerSeeder.LoadPlayers(playersPath);

        context.Players.AddRange(players);

        context.SaveChanges();

        Console.WriteLine(
            $"Players Imported: {context.Players.Count()}"
        );
    }
    else
    {
        Console.WriteLine(
            $"Players Already Exist: {context.Players.Count()}"
        );
    }

    // Load CSV

    if (!context.Players.Any())
    {
        var playersPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "DataFiles",
            "transfermarkt_cleaned_final11.csv"
        );

        var players = PlayerSeeder.LoadPlayers(playersPath);

        context.Players.AddRange(players);

        context.SaveChanges();

        Console.WriteLine(
            $"Players Imported: {context.Players.Count()}"
        );
    }
    else
    {
        Console.WriteLine(
            $"Players Already Exist: {context.Players.Count()}"
        );
    }


}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Dashboard}/{action=Index}/{id?}"
);

app.Run();
