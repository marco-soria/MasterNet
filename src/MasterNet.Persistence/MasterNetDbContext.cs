using MasterNet.Domain;
using MasterNet.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Persistence;

/// <summary>
/// Contexto de base de datos principal para la aplicación MasterNet.
/// Maneja todas las entidades del dominio y sus configuraciones.
/// Integra ASP.NET Core Identity para autenticación y autorización.
/// </summary>
public class MasterNetDbContext : IdentityDbContext<AppUser>
{
    // DbSets - Representan las tablas en la base de datos
    // Cada DbSet mapea a una tabla específica y permite realizar operaciones CRUD

    /// <summary>
    /// DbSet para la entidad Course. Representa la tabla 'courses' en la base de datos.
    /// El nombre de la propiedad (Courses) se usa por convención para generar el nombre de la tabla,
    /// pero se puede sobreescribir en OnModelCreating usando ToTable().
    /// </summary>
    public DbSet<Course>? Courses { get; set; }

    /// <summary>
    /// DbSet para la entidad Instructor. Representa la tabla 'instructors' en la base de datos.
    /// </summary>
    public DbSet<Instructor>? Instructors { get; set; }

    /// <summary>
    /// DbSet para la entidad Price. Representa la tabla 'prices' en la base de datos.
    /// </summary>
    public DbSet<Price>? Prices { get; set; }

    /// <summary>
    /// DbSet para la entidad Rating. Representa la tabla 'ratings' en la base de datos.
    /// </summary>
    public DbSet<Rating>? Ratings { get; set; }

    /// <summary>
    /// Configura la conexión a la base de datos y las opciones de logging.
    /// Este método se ejecuta cuando EF Core necesita configurar el contexto.
    /// </summary>
    /// <param name="optionsBuilder">Constructor de opciones para configurar el contexto</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuración de SQLite como proveedor de base de datos
        // SQLite es ideal para desarrollo y aplicaciones pequeñas a medianas
        optionsBuilder.UseSqlite("Data Source=LocalDatabase.db")
            .LogTo(
                Console.WriteLine, // Los logs se envían a la consola
                new[] { DbLoggerCategory.Database.Command.Name }, // Solo loggea comandos SQL
                Microsoft.Extensions.Logging.LogLevel.Information // Nivel de logging informativo
            )
            .EnableSensitiveDataLogging(); // Permite ver valores de parámetros en los logs (solo para desarrollo)
    }

    /// <summary>
    /// Configura el modelo de base de datos, incluyendo las relaciones entre entidades,
    /// nombres de tablas, precisión de campos, y datos de prueba (seed data).
    /// Este método se ejecuta cuando EF Core construye el modelo de base de datos.
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo para configurar entidades y relaciones</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CONFIGURACIÓN DE ASP.NET CORE IDENTITY
        // base.OnModelCreating(modelBuilder) ya configuró las tablas de Identity:
        // - AspNetUsers (usuarios)
        // - AspNetRoles (roles) 
        // - AspNetUserRoles (relación usuario-rol)
        // - AspNetUserClaims (claims de usuarios)
        // - AspNetRoleClaims (claims de roles)
        // - AspNetUserLogins (logins externos como Google, Facebook)
        // - AspNetUserTokens (tokens de reset password, etc.)

        // CONFIGURACIÓN PERSONALIZADA DE IDENTITY TABLES
        // Personalizar nombres de tablas de Identity para consistencia con nuestro esquema
        modelBuilder.Entity<AppUser>().ToTable("app_users");
        modelBuilder.Entity<IdentityRole>().ToTable("app_roles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("app_user_roles");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("app_user_claims");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("app_role_claims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("app_user_logins");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("app_user_tokens");

        // CONFIGURACIÓN DE CAMPOS PERSONALIZADOS DE APPUSER
        // Configurar las propiedades adicionales que agregamos a AppUser
        modelBuilder.Entity<AppUser>()
            .Property(u => u.FullName)
            .HasMaxLength(200)
            .IsRequired(); // Es obligatorio como definimos

        modelBuilder.Entity<AppUser>()
            .Property(u => u.Degree)
            .HasMaxLength(300)
            .IsRequired(false); // Es opcional como definimos

        // CONFIGURACIÓN DE NOMBRES DE TABLAS DE ENTIDADES DE DOMINIO
        // ToTable() permite especificar el nombre exacto de la tabla en la base de datos
        // Es una BUENA PRÁCTICA usar nombres en minúsculas para compatibilidad entre diferentes sistemas
        modelBuilder.Entity<Course>().ToTable("courses");
        modelBuilder.Entity<Instructor>().ToTable("instructors");
        modelBuilder.Entity<CourseInstructor>().ToTable("course_instructors");
        modelBuilder.Entity<Price>().ToTable("prices");
        modelBuilder.Entity<CoursePrice>().ToTable("course_prices");
        modelBuilder.Entity<Rating>().ToTable("ratings");
        modelBuilder.Entity<Photo>().ToTable("photos");

        // CONFIGURACIÓN DE PRECISIÓN PARA CAMPOS MONETARIOS
        modelBuilder.Entity<Price>()
            .Property(b => b.CurrentPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Price>()
            .Property(b => b.PromotionalPrice)
            .HasPrecision(10, 2);

        // CONFIGURACIÓN DE CAMPOS DE TEXTO
        modelBuilder.Entity<Price>()
            .Property(b => b.Name)
            .HasColumnType("VARCHAR")
            .HasMaxLength(250)
            .IsRequired();

        // Configuración de campos requeridos
        modelBuilder.Entity<Course>()
            .Property(c => c.Title)
            .HasMaxLength(500)
            .IsRequired();

        modelBuilder.Entity<Course>()
            .Property(c => c.Description)
            .HasColumnType("TEXT")
            .IsRequired();

        modelBuilder.Entity<Instructor>()
            .Property(i => i.FirstName)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<Instructor>()
            .Property(i => i.LastName)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<Rating>()
            .Property(r => r.Student)
            .HasMaxLength(250)
            .IsRequired();

        modelBuilder.Entity<Photo>()
            .Property(p => p.Url)
            .HasMaxLength(2000)
            .IsRequired();

        // Configuración para campos opcionales
        modelBuilder.Entity<Instructor>()
            .Property(i => i.Degree)
            .HasMaxLength(300)
            .IsRequired(false);

        modelBuilder.Entity<Rating>()
            .Property(r => r.Comment)
            .HasColumnType("TEXT")
            .IsRequired(false);

        // CONFIGURACIÓN DE RELACIONES UNO-A-MUCHOS
        modelBuilder.Entity<Course>()
            .HasMany(m => m.Photos)
            .WithOne(m => m.Course)
            .HasForeignKey(m => m.CourseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasMany(m => m.Ratings)
            .WithOne(m => m.Course)
            .HasForeignKey(m => m.CourseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de índices
        modelBuilder.Entity<Rating>()
            .HasIndex(r => r.CourseId)
            .HasDatabaseName("IX_Ratings_CourseId");

        modelBuilder.Entity<Photo>()
            .HasIndex(p => p.CourseId)
            .HasDatabaseName("IX_Photos_CourseId");

        // CONFIGURACIÓN DE RELACIONES MUCHOS-A-MUCHOS
        modelBuilder.Entity<Course>()
            .HasMany(m => m.Prices)
            .WithMany(m => m.Courses)
            .UsingEntity<CoursePrice>(
                j => j
                    .HasOne(p => p.Price)
                    .WithMany(p => p.CoursePrices)
                    .HasForeignKey(p => p.PriceId)
                    .IsRequired(),
                j => j
                    .HasOne(p => p.Course)
                    .WithMany(p => p.CoursePrices)
                    .HasForeignKey(p => p.CourseId)
                    .IsRequired(),
                j =>
                {
                    j.HasKey(t => new { t.PriceId, t.CourseId });
                    j.HasIndex(t => t.CourseId).HasDatabaseName("IX_CoursePrice_CourseId");
                    j.HasIndex(t => t.PriceId).HasDatabaseName("IX_CoursePrice_PriceId");
                }
            );

        modelBuilder.Entity<Course>()
            .HasMany(m => m.Instructors)
            .WithMany(m => m.Courses)
            .UsingEntity<CourseInstructor>(
                j => j
                    .HasOne(p => p.Instructor)
                    .WithMany(p => p.CourseInstructors)
                    .HasForeignKey(p => p.InstructorId)
                    .IsRequired(),
                j => j
                    .HasOne(p => p.Course)
                    .WithMany(p => p.CourseInstructors)
                    .HasForeignKey(p => p.CourseId)
                    .IsRequired(),
                j =>
                {
                    j.HasKey(t => new { t.InstructorId, t.CourseId });
                    j.HasIndex(t => t.CourseId).HasDatabaseName("IX_CourseInstructor_CourseId");
                    j.HasIndex(t => t.InstructorId).HasDatabaseName("IX_CourseInstructor_InstructorId");
                }
            );

        // DATOS DE PRUEBA (SEED DATA)
        LoadDomainData(modelBuilder);
        LoadSecurityData(modelBuilder);
    }

    /// <summary>
    /// Carga datos de prueba para las entidades del dominio.
    /// IMPORTANTE: Usa valores estáticos para evitar problemas de migración.
    /// </summary>
    private void LoadDomainData(ModelBuilder modelBuilder)
    {
        var courses = new Course[]
        {
            new Course
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                Title = "Complete ASP.NET Core Web Development",
                Description = "Master modern web development with ASP.NET Core, from basics to advanced concepts including MVC, Web API, and Entity Framework Core.",
                PublicationDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new Course
            {
                Id = new Guid("22222222-2222-2222-2222-222222222222"),
                Title = "Advanced Entity Framework Core",
                Description = "Deep dive into Entity Framework Core with advanced querying, performance optimization, and database design patterns.",
                PublicationDate = new DateTime(2024, 2, 20, 14, 30, 0, DateTimeKind.Utc)
            },
            new Course
            {
                Id = new Guid("33333333-3333-3333-3333-333333333333"),
                Title = "Clean Architecture with .NET",
                Description = "Learn to implement Clean Architecture principles in .NET applications for maintainable and scalable software.",
                PublicationDate = new DateTime(2024, 3, 10, 9, 15, 0, DateTimeKind.Utc)
            }
        };

        var prices = new Price[]
        {
            new Price
            {
                Id = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                Name = "Basic Tier",
                CurrentPrice = 49.99m,
                PromotionalPrice = 39.99m
            },
            new Price
            {
                Id = new Guid("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                Name = "Standard Tier",
                CurrentPrice = 99.99m,
                PromotionalPrice = 79.99m
            }
        };

        var instructors = new Instructor[]
        {
            new Instructor
            {
                Id = new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"),
                FirstName = "John",
                LastName = "Anderson",
                Degree = "Master of Computer Science"
            },
            new Instructor
            {
                Id = new Guid("12345678-1234-1234-1234-123456789012"),
                FirstName = "Sarah",
                LastName = "Williams",
                Degree = "PhD in Software Engineering"
            }
        };

        modelBuilder.Entity<Course>().HasData(courses);
        modelBuilder.Entity<Price>().HasData(prices);
        modelBuilder.Entity<Instructor>().HasData(instructors);
    }

    /// <summary>
    /// Carga datos de seguridad iniciales: roles, claims y políticas.
    /// IMPORTANTE: Usa IDs estáticos para evitar problemas de migración.
    /// </summary>
    private void LoadSecurityData(ModelBuilder modelBuilder)
    {
        // IDs ESTÁTICOS para evitar problemas de migración
        var adminId = "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012";
        var clientId = "CLIENT-ROLE-ID-87654321-4321-4321-4321-210987654321";

        // CREACIÓN DE ROLES BÁSICOS
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole 
            {
                Id = adminId,
                Name = CustomRoles.ADMIN,
                NormalizedName = CustomRoles.ADMIN,
                ConcurrencyStamp = "ADMIN-STAMP-12345"
            },
            new IdentityRole 
            {
                Id = clientId,
                Name = CustomRoles.CLIENT,
                NormalizedName = CustomRoles.CLIENT,
                ConcurrencyStamp = "CLIENT-STAMP-54321"
            }
        );

        // ASIGNACIÓN DE CLAIMS/POLÍTICAS A ROLES
        modelBuilder.Entity<IdentityRoleClaim<string>>().HasData(
            // Claims para ADMIN - Permisos completos
            new IdentityRoleClaim<string>
            {
                Id = 1,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.COURSE_READ,
                RoleId = adminId
            },
            new IdentityRoleClaim<string>
            {
                Id = 2,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.COURSE_WRITE,
                RoleId = adminId
            },
            new IdentityRoleClaim<string>
            {
                Id = 3,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.COURSE_UPDATE,
                RoleId = adminId
            },
            new IdentityRoleClaim<string>
            {
                Id = 4,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.COURSE_DELETE,
                RoleId = adminId
            },
            new IdentityRoleClaim<string>
            {
                Id = 5,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.INSTRUCTOR_READ,
                RoleId = adminId
            },
            new IdentityRoleClaim<string>
            {
                Id = 6,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.INSTRUCTOR_CREATE,
                RoleId = adminId
            },
            new IdentityRoleClaim<string>
            {
                Id = 7,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.INSTRUCTOR_UPDATE,
                RoleId = adminId
            },
            new IdentityRoleClaim<string>
            {
                Id = 8,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.COMMENT_READ,
                RoleId = adminId
            },
            new IdentityRoleClaim<string>
            {
                Id = 9,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.COMMENT_CREATE,
                RoleId = adminId
            },
            new IdentityRoleClaim<string>
            {
                Id = 10,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.COMMENT_DELETE,
                RoleId = adminId
            },

            // Claims para CLIENT - Permisos básicos
            new IdentityRoleClaim<string>
            {
                Id = 11,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.COURSE_READ,
                RoleId = clientId
            },
            new IdentityRoleClaim<string>
            {
                Id = 12,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.INSTRUCTOR_READ,
                RoleId = clientId
            },
            new IdentityRoleClaim<string>
            {
                Id = 13,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.COMMENT_READ,
                RoleId = clientId
            },
            new IdentityRoleClaim<string>
            {
                Id = 14,
                ClaimType = CustomClaims.POLICIES,
                ClaimValue = PolicyMaster.COMMENT_CREATE,
                RoleId = clientId
            }
        );
    }
}
