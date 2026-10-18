using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Office.Application.Common.Interfaces;
using Office.Infrastructure.Data;
using Office.Infrastructure.Repositories;

namespace Office.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OfficeDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection")));

        services.AddScoped<IOfficeDbContext>(provider => provider.GetRequiredService<OfficeDbContext>());
        services.AddScoped<IOfficeRepository, OfficeRepository>();

        return services;
    }
}
