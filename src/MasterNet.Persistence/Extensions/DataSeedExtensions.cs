using MasterNet.Domain;
using MasterNet.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MasterNet.Persistence.Extensions;

/// <summary>
/// Extensiones para el seeding de datos en la aplicación.
/// Ubicado en Persistence layer siguiendo Clean Architecture.
/// </summary>
public static class DataSeedExtensions
{
    /// <summary>
    /// Versión optimizada del seed que evita queries innecesarias si ya existen datos.
    /// Ideal para evitar modificaciones constantes de la base de datos SQLite.
    /// </summary>
    public static async Task SeedDataAsync(
        MasterNetDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger logger
    )
    {
        try
        {
            // Verificación rápida: si ya hay cursos, asumimos que el seed ya se ejecutó
            var hasData = await context.Courses!.AnyAsync();
            if (hasData)
            {
                logger.LogInformation("Data already exists. Skipping seed.");
                return;
            }

            logger.LogInformation("No data found. Starting seed process...");

            // Seed roles - SIEMPRE necesario (todos los environments)
            await SeedRolesAsync(roleManager, logger);

            // Seed usuario admin esencial - SIEMPRE necesario para acceso inicial
            await SeedEssentialUsersAsync(userManager, roleManager, logger);

            // Seed de datos según environment
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var seedSampleData = Environment.GetEnvironmentVariable("SEED_SAMPLE_DATA");
            
            // Determinar si seedear datos de muestra
            bool shouldSeedSampleData = environment switch
            {
                "Development" => true,                    // ✅ Development: siempre datos de muestra
                "Staging" => true,                        // ✅ Staging: datos de muestra para testing
                "Production" => seedSampleData == "true", // ❓ Production: solo si está explícitamente habilitado
                _ => seedSampleData == "true"             // ❓ Otros: solo si está explícitamente habilitado
            };
            
            if (shouldSeedSampleData)
            {
                logger.LogInformation($"Environment: {environment}. Seeding sample data...");
                await SeedDevelopmentDataAsync(context, userManager, roleManager, logger);
            }
            else
            {
                logger.LogInformation($"Environment: {environment ?? "Unknown"}. Skipping sample data seeding.");
                logger.LogInformation("Tip: Set environment variable SEED_SAMPLE_DATA=true to include sample data.");
            }
            
            logger.LogInformation("Data seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding data");
            throw;
        }
    }

    /// <summary>
    /// Seedea datos de autenticación y datos base para desarrollo.
    /// Utiliza datos estáticos para evitar problemas en migraciones.
    /// </summary>
    public static async Task SeedDataAsync(
        this IServiceProvider serviceProvider,
        ILogger logger
    )
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MasterNetDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Asegurar que la base de datos esté actualizada
            await context.Database.MigrateAsync();

            // Usar la versión optimizada
            await SeedDataAsync(context, userManager, roleManager, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding data");
            throw;
        }
    }

    private static async Task SeedEssentialUsersAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        // Verificar si ya existe un usuario admin
        var existingAdmins = await userManager.GetUsersInRoleAsync(CustomRoles.ADMIN);
        if (existingAdmins.Any())
        {
            logger.LogInformation("Admin user already exists. Skipping essential user creation.");
            return;
        }

        logger.LogInformation("Creating essential admin user for system access...");

        // Usuario Administrador ESENCIAL - SIEMPRE necesario
        var adminUser = new AppUser
        {
            FullName = "Admin Admin",
            UserName = "admin",
            Email = "admin@gmail.com",
            EmailConfirmed = true,
            Degree = "System Administration"
        };

        var adminResult = await userManager.CreateAsync(adminUser, "Admin123$");
        if (adminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, CustomRoles.ADMIN);
            logger.LogInformation("Essential admin user created successfully");
            logger.LogInformation("Login credentials - Email: admin@masternet.com, Password: Admin123$");
        }
        else
        {
            logger.LogError($"Failed to create essential admin user: {string.Join(", ", adminResult.Errors.Select(e => e.Description))}");
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        logger.LogInformation("Seeding roles...");

        // Crear rol ADMIN si no existe
        if (!await roleManager.RoleExistsAsync(CustomRoles.ADMIN))
        {
            var adminRole = new IdentityRole(CustomRoles.ADMIN);
            var result = await roleManager.CreateAsync(adminRole);
            if (result.Succeeded)
            {
                logger.LogInformation($"Role {CustomRoles.ADMIN} created successfully");
            }
            else
            {
                logger.LogError($"Failed to create role {CustomRoles.ADMIN}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // Crear rol CLIENT si no existe
        if (!await roleManager.RoleExistsAsync(CustomRoles.CLIENT))
        {
            var clientRole = new IdentityRole(CustomRoles.CLIENT);
            var result = await roleManager.CreateAsync(clientRole);
            if (result.Succeeded)
            {
                logger.LogInformation($"Role {CustomRoles.CLIENT} created successfully");
            }
            else
            {
                logger.LogError($"Failed to create role {CustomRoles.CLIENT}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }

    private static async Task SeedSampleUsersAsync(UserManager<AppUser> userManager, ILogger logger)
    {
        // Solo crear usuarios de muestra si no existen
        var sampleClientEmail = "johnsmith@gmail.com";
        var existingClient = await userManager.FindByEmailAsync(sampleClientEmail);
        
        if (existingClient == null)
        {
            logger.LogInformation("Creating sample users for development/testing...");

            // Usuario Cliente de MUESTRA - Solo para development/staging
            var clientUser = new AppUser
            {
                FullName = "John Smith",
                UserName = "johnsmith",
                Email = sampleClientEmail,
                EmailConfirmed = true,
                Degree = "Software Engineering"
            };

            var clientResult = await userManager.CreateAsync(clientUser, "Client123$");
            if (clientResult.Succeeded)
            {
                await userManager.AddToRoleAsync(clientUser, CustomRoles.CLIENT);
                logger.LogInformation("Sample client user created successfully");
            }
        }
    }

    private static async Task SeedDomainDataAsync(MasterNetDbContext context, ILogger logger)
    {
        // Seed Instructors
        if (!context.Instructors!.Any())
        {
            logger.LogInformation("Seeding instructors...");
            
            var instructors = new List<Instructor>
            {
                new() {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Degree = "Ph.D. in Computer Science"
                },
                new() {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    FirstName = "Michael",
                    LastName = "Chen",
                    Degree = "Master in Software Engineering"
                },
                new() {
                    Id = new Guid("33333333-3333-3333-3333-333333333333"),
                    FirstName = "Emily",
                    LastName = "Rodriguez",
                    Degree = "Master in Data Science"
                }
            };

            context.Instructors!.AddRange(instructors);
        }

        // Seed Prices
        if (!context.Prices!.Any())
        {
            logger.LogInformation("Seeding prices...");
            
            var prices = new List<Price>
            {
                new() {
                    Id = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                    Name = "Basic Course",
                    CurrentPrice = 29.99m,
                    PromotionalPrice = 19.99m
                },
                new() {
                    Id = new Guid("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                    Name = "Premium Course",
                    CurrentPrice = 59.99m,
                    PromotionalPrice = 39.99m
                },
                new() {
                    Id = new Guid("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
                    Name = "Professional Course",
                    CurrentPrice = 99.99m,
                    PromotionalPrice = 79.99m
                }
            };

            context.Prices!.AddRange(prices);
        }

        // Seed Courses
        if (!context.Courses!.Any())
        {
            logger.LogInformation("Seeding courses...");
            
            var courses = new List<Course>
            {
                new() {
                    Id = new Guid("44444444-4444-4444-4444-444444444444"),
                    Title = "Introduction to Programming",
                    Description = "Learn the fundamentals of programming with practical examples and hands-on exercises.",
                    PublicationDate = DateTime.UtcNow.AddDays(-30)
                },
                new() {
                    Id = new Guid("55555555-5555-5555-5555-555555555555"),
                    Title = "Advanced Web Development",
                    Description = "Master modern web development techniques using the latest frameworks and tools.",
                    PublicationDate = DateTime.UtcNow.AddDays(-15)
                },
                new() {
                    Id = new Guid("66666666-6666-6666-6666-666666666666"),
                    Title = "Data Science Fundamentals",
                    Description = "Explore data analysis, machine learning, and statistical methods for data-driven decisions.",
                    PublicationDate = DateTime.UtcNow.AddDays(-7)
                }
            };

            context.Courses!.AddRange(courses);
        }

        await context.SaveChangesAsync();

        // Seed relationships after base entities are saved
        await SeedRelationshipsAsync(context, logger);
    }

    private static async Task SeedRelationshipsAsync(MasterNetDbContext context, ILogger logger)
    {
        // Associate instructors with courses
        if (!context.Set<CourseInstructor>().Any())
        {
            logger.LogInformation("Seeding course-instructor relationships...");
            
            var courses = await context.Courses!.ToListAsync();
            var instructors = await context.Instructors!.ToListAsync();

            if (courses.Any() && instructors.Any())
            {
                // Ensure we have enough data to work with
                if (courses.Count >= 1 && instructors.Count >= 1)
                {
                    courses[0].Instructors.Add(instructors[0]); // First course with first instructor
                }
                
                if (courses.Count >= 2 && instructors.Count >= 2)
                {
                    courses[1].Instructors.Add(instructors[1]); // Second course with second instructor
                }
                
                if (courses.Count >= 3 && instructors.Count >= 2)
                {
                    courses[2].Instructors.Add(instructors[1]); // Third course also with second instructor
                }
                
                // Some courses can have multiple instructors
                if (courses.Count >= 2 && instructors.Count >= 1)
                {
                    courses[1].Instructors.Add(instructors[0]); // Second course also with first instructor
                }
            }
        }

        // Associate prices with courses
        if (!context.Set<CoursePrice>().Any())
        {
            logger.LogInformation("Seeding course-price relationships...");
            
            var courses = await context.Courses!.ToListAsync();
            var prices = await context.Prices!.ToListAsync();

            if (courses.Any() && prices.Any())
            {
                // Ensure we have enough data to work with
                if (courses.Count >= 1 && prices.Count >= 1)
                {
                    courses[0].Prices.Add(prices[0]); // First course - first price
                }
                
                if (courses.Count >= 2 && prices.Count >= 2)
                {
                    courses[1].Prices.Add(prices[1]); // Second course - second price
                }
                
                if (courses.Count >= 3 && prices.Count >= 1)
                {
                    courses[2].Prices.Add(prices[0]); // Third course - first price
                }
            }
        }

        // Seed sample ratings
        if (!context.Set<Rating>().Any())
        {
            logger.LogInformation("Seeding sample ratings...");
            
            var courses = await context.Courses!.ToListAsync();
            
            if (courses.Any())
            {
                var ratings = new List<Rating>();
                
                // Only add ratings for courses that exist
                if (courses.Count >= 1)
                {
                    ratings.AddRange(new[]
                    {
                        new Rating {
                            Id = Guid.NewGuid(),
                            Student = "Alice Thompson",
                            Comment = "Excellent course! Very clear explanations and great examples.",
                            Score = 5,
                            CourseId = courses[0].Id
                        },
                        new Rating {
                            Id = Guid.NewGuid(),
                            Student = "Robert Davis",
                            Comment = "Good content, but could use more practical exercises.",
                            Score = 4,
                            CourseId = courses[0].Id
                        }
                    });
                }
                
                if (courses.Count >= 2)
                {
                    ratings.Add(new Rating {
                        Id = Guid.NewGuid(),
                        Student = "Maria Garcia",
                        Comment = "Outstanding course! Highly recommended for beginners.",
                        Score = 5,
                        CourseId = courses[1].Id
                    });
                }

                if (ratings.Any())
                {
                    context.Set<Rating>().AddRange(ratings);
                }
            }
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Método para seedear datos de desarrollo únicamente.
    /// Se llama solo cuando ASPNETCORE_ENVIRONMENT = Development.
    /// </summary>
    private static async Task SeedDevelopmentDataAsync(
        MasterNetDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger logger)
    {
        await SeedSampleUsersAsync(userManager, logger);
        await SeedDomainDataAsync(context, logger);
    }
}
