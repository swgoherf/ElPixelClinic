using Microsoft.Extensions.DependencyInjection;
using Services.Application.Services;

namespace Services.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<ISpecializationService, SpecializationService>();
        return services;
    }
}
