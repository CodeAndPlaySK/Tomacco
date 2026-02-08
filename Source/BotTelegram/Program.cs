using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Telegram.Bot;
using TelegramBot.Extensions;

// Configurazione Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/startup-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

Log.Information("🚀 Avvio applicazione...");

try
{
    var builder = Host.CreateApplicationBuilder(args);

    // Configurazione
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddUserSecrets<Program>(optional: true)
        .AddEnvironmentVariables();


    // Configurazione Serilog dal file di configurazione
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/game-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30
        ));

    // Leggi il token dalla configurazione
    var botToken = builder.Configuration["Telegram:BotToken"];

    if (string.IsNullOrEmpty(botToken))
    {
        Log.Fatal("❌ Token del bot non configurato!");
        Console.WriteLine("❌ ERRORE: Token del bot non configurato!");
        Console.WriteLine("");
        Console.WriteLine("Configura il token in uno dei seguenti modi:");
        Console.WriteLine("1. User Secrets: dotnet user-secrets set \"Telegram:BotToken\" \"YOUR_TOKEN\"");
        Console.WriteLine("2. appsettings.Development.json: copia appsettings.Development.json.template");
        Console.WriteLine("3. Variabile d'ambiente: TELEGRAM__BOTTOKEN");
        return;
    }

    // Registrazione servizi tramite extensions
    builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddTelegramBot();

    var host = builder.Build();

    // Crea il database se non esiste
    using (var scope = host.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Inizializzazione database...");

        var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        logger.LogInformation("✅ Database inizializzato");
        Console.WriteLine("✅ Database inizializzato");
    }

    Log.Information("🤖 Bot Telegram avviato!");
    Console.WriteLine("🤖 Bot Telegram avviato!");
    Console.WriteLine("📊 Logs salvati in: logs/");
    Console.WriteLine("Premi CTRL+C per fermare il bot");

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "💥 Errore fatale durante l'avvio dell'applicazione");
    Console.WriteLine($"💥 Errore fatale: {ex.Message}");
}
finally
{
    Log.Information("👋 Arresto applicazione");
    await Log.CloseAndFlushAsync();
}