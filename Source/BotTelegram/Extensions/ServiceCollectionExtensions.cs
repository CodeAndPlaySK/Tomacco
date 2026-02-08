using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Services;
using BotTelegram.Handlers.SubHandlers;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Handlers.Commands.Game;
using TelegramBot.Handlers.Commands.Player;
using TelegramBot.Handlers.Commands.Systrem;
using Infrastructure.Repositories;
using TelegramBot.Services;
using Domain.Factories;
using TelegramBot.Handlers.Commands.City;
using TelegramBot.Handlers.Commands.Family;

namespace TelegramBot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["Database:ConnectionString"] ?? "Data Source=game.db";

            services.AddDbContext<GameDbContext>(options =>
                options.UseSqlite(connectionString));

            // Repository Registration
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IGameRepository, GameRepository>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Factories
            services.AddScoped<ICityFactory, CityFactory>();
            services.AddScoped<IFamilyFactory, FamilyFactory>();
            services.AddScoped<IHeroFactory, HeroFactory>();

            // Services
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();

            return services;
        }

        public static IServiceCollection AddTelegramBot(this IServiceCollection services)
        {
            services.AddSingleton<CommandRegistry>();

            // Player Commands
            services.AddScoped<StartCommandHandler>();
            services.AddScoped<ProfileCommandHandler>();
            services.AddScoped<ListPlayersCommandHandler>();
            services.AddScoped<UpdateNameCommandHandler>();

            // Game Commands
            services.AddScoped<CreateGameCommandHandler>();
            services.AddScoped<ListGamesCommandHandler>();
            services.AddScoped<JoinGameCommandHandler>();
            services.AddScoped<MyGamesCommandHandler>();
            services.AddScoped<StartGameCommandHandler>();
            services.AddScoped<EndGameCommandHandler>();

            // Game Info Commands
            services.AddScoped<GameInfoCommandHandler>();
            services.AddScoped<CityInfoCommandHandler>();
            services.AddScoped<FamilyInfoCommandHandler>();

            // System Commands
            services.AddScoped<HelpCommandHandler>();
            services.AddScoped<LanguageCommandHandler>();
            services.AddScoped<SetLanguageCommandHandler>();
            services.AddScoped<AuditCommandHandler>();

            services.AddHostedService<TelegramBotService>();

            return services;
        }
    }
}
