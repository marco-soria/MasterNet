using MasterNet.Persistence.Extensions;
using MasterNet.Persistence.Interfaces;
using MasterNet.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MasterNet.Persistence.Services;

/// <summary>
/// Implementación del servicio de migración de base de datos.
/// Maneja migraciones automáticas y seeding de datos iniciales.
/// </summary>
public class DatabaseMigrationService : IDatabaseMigrationService
{
    private readonly MasterNetDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<DatabaseMigrationService> _logger;

    public DatabaseMigrationService(
        MasterNetDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<DatabaseMigrationService> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<bool> CanConnectAsync()
    {
        try
        {
            return await _context.Database.CanConnectAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to database");
            return false;
        }
    }

    public async Task MigrateAsync()
    {
        try
        {
            _logger.LogInformation("🔄 Checking for pending database migrations...");
            
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("📦 Found {Count} pending migrations. Applying...", pendingMigrations.Count());
                await _context.Database.MigrateAsync();
                _logger.LogInformation("✅ Database migrations applied successfully");
            }
            else
            {
                _logger.LogInformation("✅ Database is up to date. No migrations needed.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error applying database migrations");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("🌱 Starting database seeding...");
            await DataSeedExtensions.SeedDataAsync(_context, _userManager, _roleManager, _logger);
            _logger.LogInformation("✅ Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during database seeding");
            throw;
        }
    }
}
