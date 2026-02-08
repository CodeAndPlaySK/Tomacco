using Application.Interfaces;
using Application.Services;
using Domain.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Bootstrap
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddGameServices(this IServiceCollection services)
        {
            services.AddDbContext<GameDbContext>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IDevelopmentService, DevelopmentService>();
            services.AddScoped<IFamilyOfPlayerService, FamilyOfPlayerService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IPlayerService, PlayerService>();
            
            // Repository Registration
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IGameRepository, GameRepository>();

            services.AddScoped<IMoveResolverService, MoveResolverService>();
            services.AddScoped<IMoveApplicationService, MoveApplicationService>();
            services.AddScoped<ICityNameGenerator, CityNameGenerator>();

            // services.AddScoped<IFamilyService, FamilyService>();
            // services.AddScoped<IGameService, GameService>();

            return services;
        }
    }
}
