using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PCM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Validators (FluentValidation)
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Application services (stubs/implementations)
        services.AddScoped<PCM.Application.Services.BookingService>();
        services.AddScoped<PCM.Application.Services.WalletService>();
        services.AddScoped<PCM.Application.Services.MatchService>();
        services.AddScoped<PCM.Application.Services.TournamentService>();


        return services;
    }
}
