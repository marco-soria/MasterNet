using MasterNet.Persistence.Interfaces;
using MasterNet.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MasterNet.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<MasterNetDbContext>(opt => {
            opt.UseSqlite(configuration.GetConnectionString("SqliteDatabase"));
        });

        // ✅ Registrar servicio de migración
        services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();

        return services;
    }
}