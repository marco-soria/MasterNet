using MasterNet.Persistence.Interfaces;
using Microsoft.Extensions.Logging;

namespace MasterNet.WebApi.Extensions;

/// <summary>
/// Extensiones para IApplicationBuilder que manejan la inicialización de base de datos.
/// Proporciona una interfaz limpia para aplicar migraciones y seeding.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Aplica migraciones automáticas y seeding de datos de forma segura.
    /// Incluye manejo de errores y logging detallado.
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Task para operación async</returns>
    public static async Task ApplyDatabaseInitializationAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var migrationService = serviceProvider.GetRequiredService<IDatabaseMigrationService>();
            
            // 1. Verificar conectividad
            logger.LogInformation("🔍 Verifying database connectivity...");
            var canConnect = await migrationService.CanConnectAsync();
            if (!canConnect)
            {
                throw new InvalidOperationException("Cannot connect to database. Check connection string and database server.");
            }
            
            // 2. Aplicar migraciones
            await migrationService.MigrateAsync();
            
            // 3. Aplicar seeding
            await migrationService.SeedAsync();
            
            logger.LogInformation("🎉 Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "💥 Critical error during database initialization");
            
            // Re-lanzar para que la aplicación no inicie con BD en mal estado
            throw;
        }
    }
}
