using Application.Interfaces;
using Application.Services;
using Console.Menus;
using Console.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Domain.Factories;
using System.Text;

System.Console.OutputEncoding = Encoding.UTF8;
System.Console.InputEncoding = Encoding.UTF8;

// Configurazione Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/console-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    // Configurazione
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    // Serilog
    builder.Services.AddSerilog();

    // Database
    var connectionString = builder.Configuration["Database:ConnectionString"] ?? "Data Source=console_game.db";
    builder.Services.AddDbContext<GameDbContext>(options =>
        options.UseSqlite(connectionString));

    // Repositories
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
    builder.Services.AddScoped<IGameRepository, GameRepository>();

    // Factories
    builder.Services.AddScoped<ICityFactory, CityFactory>();
    builder.Services.AddScoped<IFamilyFactory, FamilyFactory>();

    // Services
    builder.Services.AddScoped<IPlayerService, PlayerService>();
    builder.Services.AddScoped<IGameService, GameService>();
    builder.Services.AddScoped<IAuditService, AuditService>();
    builder.Services.AddSingleton<ILocalizationService, ConsoleLocalizationService>();

    // Console UI
    builder.Services.AddScoped<ConsolePlayerUI>();
    builder.Services.AddScoped<ConsoleGameUI>();
    builder.Services.AddScoped<MainMenu>();

    var host = builder.Build();

    // Inizializza database
    using (var scope = host.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        Log.Information("Database inizializzato");
    }

    // Avvia l'applicazione
    using (var scope = host.Services.CreateScope())
    {
        var mainMenu = scope.ServiceProvider.GetRequiredService<MainMenu>();
        await mainMenu.RunAsync();
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Errore fatale nell'applicazione");
    AnsiConsole.WriteException(ex);
}
finally
{
    await Log.CloseAndFlushAsync();
}
