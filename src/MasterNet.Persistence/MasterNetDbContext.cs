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

    public MasterNetDbContext(DbContextOptions<MasterNetDbContext> options) : base(options)
    {
    }
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

        // NOTA: Seed data ahora se maneja a través de DataSeedExtensions
        // para seguir principios de Clean Architecture y separación de responsabilidades
    }
}
