using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PCM.Core.Interfaces;
using PCM.Infrastructure.Hubs;
using PCM.Infrastructure.Persistence;
using PCM.Infrastructure.Services;
using StackExchange.Redis;

namespace PCM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string connectionString,
        string? redisConnectionString = null)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly("PCM.Infrastructure")));

        // Unit of Work & Repository
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        // Redis Cache
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(redis);
            services.AddScoped<ICacheService, RedisCacheService>();
        }

        // Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISignalRService, SignalRService>();
        services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();

        // SignalR
        services.AddSignalR();

        return services;
    }
}
