using Microsoft.Extensions.DependencyInjection;
using Office.Application.Services;

namespace Office.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOfficeService, OfficeService>();
        return services;
    }
}
