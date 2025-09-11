namespace MasterNet.Persistence.Interfaces;

/// <summary>
/// Servicio para manejo de migraciones y seeding de base de datos.
/// Abstrae la lógica de migración para mejorar testabilidad y separación de responsabilidades.
/// </summary>
public interface IDatabaseMigrationService
{
    /// <summary>
    /// Aplica todas las migraciones pendientes a la base de datos.
    /// </summary>
    Task MigrateAsync();
    
    /// <summary>
    /// Ejecuta el seeding de datos iniciales (usuarios, roles, datos de dominio).
    /// </summary>
    Task SeedAsync();
    
    /// <summary>
    /// Verifica si la base de datos puede conectarse correctamente.
    /// </summary>
    Task<bool> CanConnectAsync();
}
