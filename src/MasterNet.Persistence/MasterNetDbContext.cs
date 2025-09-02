using MasterNet.Domain;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Persistence;

/// <summary>
/// Contexto de base de datos principal para la aplicación MasterNet.
/// Maneja todas las entidades del dominio y sus configuraciones.
/// </summary>
public class MasterNetDbContext : DbContext
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

        // CONFIGURACIÓN DE NOMBRES DE TABLAS
        // ToTable() permite especificar el nombre exacto de la tabla en la base de datos
        // Es una BUENA PRÁCTICA usar nombres en minúsculas para compatibilidad entre diferentes sistemas
        // DIFERENCIAS entre definir aquí vs el nombre del DbSet:
        // - DbSet<Course> Courses: El nombre 'Courses' es usado por EF por convención (pluralizado)
        // - ToTable("courses"): Sobreescribe la convención y usa exactamente "courses" (minúsculas)
        // - Sin ToTable(): EF usaría "Courses" (con mayúscula inicial) por convención

        modelBuilder.Entity<Course>().ToTable("courses");
        modelBuilder.Entity<Instructor>().ToTable("instructors");
        modelBuilder.Entity<CourseInstructor>().ToTable("course_instructors");
        modelBuilder.Entity<Price>().ToTable("prices");
        modelBuilder.Entity<CoursePrice>().ToTable("course_prices");
        modelBuilder.Entity<Rating>().ToTable("ratings");
        modelBuilder.Entity<Photo>().ToTable("photos");

        // CONFIGURACIÓN DE PRECISIÓN PARA CAMPOS MONETARIOS
        // Los campos decimal requieren precisión específica para manejo correcto de dinero
        // HasPrecision(10, 2) significa: 10 dígitos totales, 2 después del punto decimal
        // Ejemplo: 12345678.99 (máximo valor posible)

        modelBuilder.Entity<Price>()
            .Property(b => b.CurrentPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Price>()
            .Property(b => b.PromotionalPrice)
            .HasPrecision(10, 2);

        // CONFIGURACIÓN DE CAMPOS DE TEXTO
        // VARCHAR con longitud máxima para optimización de almacenamiento y rendimiento
        modelBuilder.Entity<Price>()
            .Property(b => b.Name)
            .HasColumnType("VARCHAR")
            .HasMaxLength(250)
            .IsRequired(); // Ahora es required porque cambió de string? a string

        // Configuración de campos requeridos que cambiaron de nullable a non-nullable
        modelBuilder.Entity<Course>()
            .Property(c => c.Title)
            .HasMaxLength(500)
            .IsRequired(); // Era string?, ahora es string = default!

        modelBuilder.Entity<Course>()
            .Property(c => c.Description)
            .HasColumnType("TEXT")
            .IsRequired(); // Era string?, ahora es string = default!

        modelBuilder.Entity<Instructor>()
            .Property(i => i.FirstName)
            .HasMaxLength(150)
            .IsRequired(); // Era string?, ahora es string = default!

        modelBuilder.Entity<Instructor>()
            .Property(i => i.LastName)
            .HasMaxLength(150)
            .IsRequired(); // Era string?, ahora es string = default!

        modelBuilder.Entity<Rating>()
            .Property(r => r.Student)
            .HasMaxLength(250)
            .IsRequired(); // Era string?, ahora es string = default!

        modelBuilder.Entity<Photo>()
            .Property(p => p.Url)
            .HasMaxLength(2000)
            .IsRequired(); // Era string?, ahora es string = default!

        // Configuración para campos que siguen siendo opcionales
        modelBuilder.Entity<Instructor>()
            .Property(i => i.Degree)
            .HasMaxLength(300)
            .IsRequired(false); // Sigue siendo string? - opcional

        modelBuilder.Entity<Rating>()
            .Property(r => r.Comment)
            .HasColumnType("TEXT")
            .IsRequired(false); // Sigue siendo string? - opcional

        // CONFIGURACIÓN DE RELACIONES UNO-A-MUCHOS

        /// <summary>
        /// Relación Course -> Photos (uno a muchos)
        /// - Un curso puede tener muchas fotos
        /// - Una foto pertenece a un solo curso
        /// - DeleteBehavior.Cascade: Al eliminar un curso, se eliminan automáticamente sus fotos
        /// - CourseId es required: Una foto siempre debe tener un curso asociado
        /// </summary>
        modelBuilder.Entity<Course>()
            .HasMany(m => m.Photos)
            .WithOne(m => m.Course)
            .HasForeignKey(m => m.CourseId)
            .IsRequired() // CourseId no puede ser null
            .OnDelete(DeleteBehavior.Cascade);

        /// <summary>
        /// Relación Course -> Ratings (uno a muchos)
        /// - Un curso puede tener muchas calificaciones
        /// - Una calificación pertenece a un solo curso
        /// - DeleteBehavior.Restrict: Al intentar eliminar un curso con calificaciones, se produce error
        /// - Esto preserva el historial de calificaciones
        /// - CourseId es required: Una calificación siempre debe tener un curso asociado
        /// </summary>
        modelBuilder.Entity<Course>()
            .HasMany(m => m.Ratings)
            .WithOne(m => m.Course)
            .HasForeignKey(m => m.CourseId)
            .IsRequired() // CourseId no puede ser null
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración adicional para validaciones de rango
        modelBuilder.Entity<Rating>()
            .Property(r => r.Score)
            .HasComment("Score debe estar entre 1 y 5");

        // Configuración de índices para mejorar performance
        modelBuilder.Entity<Rating>()
            .HasIndex(r => r.CourseId)
            .HasDatabaseName("IX_Ratings_CourseId");

        modelBuilder.Entity<Photo>()
            .HasIndex(p => p.CourseId)
            .HasDatabaseName("IX_Photos_CourseId");

        // CONFIGURACIÓN DE RELACIONES MUCHOS-A-MUCHOS

        /// <summary>
        /// Relación muchos-a-muchos: Course <-> Price
        /// - Un curso puede tener múltiples precios (ej: precio regular, precio estudiante)
        /// - Un precio puede aplicar a múltiples cursos
        /// - UsingEntity<CoursePrice>: Especifica la entidad intermedia que maneja la relación
        /// - HasKey: Define clave primaria compuesta (PriceId + CourseId)
        /// - Ambas FK son required para la integridad referencial
        /// </summary>
        modelBuilder.Entity<Course>()
            .HasMany(m => m.Prices)
            .WithMany(m => m.Courses)
            .UsingEntity<CoursePrice>(
                j => j
                    .HasOne(p => p.Price)
                    .WithMany(p => p.CoursePrices)
                    .HasForeignKey(p => p.PriceId)
                    .IsRequired(), // PriceId no puede ser null
                j => j
                    .HasOne(p => p.Course)
                    .WithMany(p => p.CoursePrices)
                    .HasForeignKey(p => p.CourseId)
                    .IsRequired(), // CourseId no puede ser null
                j =>
                {
                    j.HasKey(t => new { t.PriceId, t.CourseId });
                    // Índices adicionales para optimización
                    j.HasIndex(t => t.CourseId).HasDatabaseName("IX_CoursePrice_CourseId");
                    j.HasIndex(t => t.PriceId).HasDatabaseName("IX_CoursePrice_PriceId");
                }
            );

        /// <summary>
        /// Relación muchos-a-muchos: Course <-> Instructor
        /// - Un curso puede tener múltiples instructores
        /// - Un instructor puede enseñar múltiples cursos
        /// - UsingEntity<CourseInstructor>: Especifica la entidad intermedia
        /// - HasKey: Define clave primaria compuesta (InstructorId + CourseId)
        /// - Ambas FK son required para la integridad referencial
        /// </summary>
        modelBuilder.Entity<Course>()
            .HasMany(m => m.Instructors)
            .WithMany(m => m.Courses)
            .UsingEntity<CourseInstructor>(
                j => j
                    .HasOne(p => p.Instructor)
                    .WithMany(p => p.CourseInstructors)
                    .HasForeignKey(p => p.InstructorId)
                    .IsRequired(), // InstructorId no puede ser null
                j => j
                    .HasOne(p => p.Course)
                    .WithMany(p => p.CourseInstructors)
                    .HasForeignKey(p => p.CourseId)
                    .IsRequired(), // CourseId no puede ser null
                j =>
                {
                    j.HasKey(t => new { t.InstructorId, t.CourseId });
                    // Índices adicionales para optimización
                    j.HasIndex(t => t.CourseId).HasDatabaseName("IX_CourseInstructor_CourseId");
                    j.HasIndex(t => t.InstructorId).HasDatabaseName("IX_CourseInstructor_InstructorId");
                }
            );

        // DATOS DE PRUEBA (SEED DATA)
        // HasData() inserta datos iniciales en la base de datos durante la migración
        // Útil para pruebas y desarrollo, pero debe usarse con cuidado en producción
        modelBuilder.Entity<Course>().HasData(DataMaster().Item1);
        modelBuilder.Entity<Price>().HasData(DataMaster().Item2);
        modelBuilder.Entity<Instructor>().HasData(DataMaster().Item3);
    }


    /// <summary>
/// Generates STATIC test data to populate the database during development.
/// IMPORTANT: Uses hardcoded values to prevent EF Core model changes.
/// Dynamic values (Guid.NewGuid(), DateTime.Now, Faker) cause migration issues.
/// </summary>
/// <returns>Tuple with Course, Price, and Instructor arrays for seed data</returns>
    public Tuple<Course[], Price[], Instructor[]> DataMaster()
    {
    // IMPORTANT: All values are STATIC to prevent model changes
    // If you need to modify this data, you must create a new migration

    // Static course data - Fixed GUIDs and dates
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
            },
            new Course
            {
                Id = new Guid("44444444-4444-4444-4444-444444444444"),
                Title = "Microservices Architecture with .NET",
                Description = "Build scalable microservices using .NET, Docker, and modern cloud technologies with hands-on projects.",
                PublicationDate = new DateTime(2024, 4, 5, 16, 45, 0, DateTimeKind.Utc)
            },
            new Course
            {
                Id = new Guid("55555555-5555-5555-5555-555555555555"),
                Title = "Unit Testing and TDD in .NET",
                Description = "Master comprehensive testing strategies including unit testing, integration testing, and test-driven development.",
                PublicationDate = new DateTime(2024, 5, 12, 11, 20, 0, DateTimeKind.Utc)
            },
            new Course
            {
                Id = new Guid("66666666-6666-6666-6666-666666666666"),
                Title = "RESTful Web APIs with ASP.NET Core",
                Description = "Design and develop professional RESTful APIs with authentication, versioning, and comprehensive documentation.",
                PublicationDate = new DateTime(2024, 6, 18, 13, 30, 0, DateTimeKind.Utc)
            },
            new Course
            {
                Id = new Guid("77777777-7777-7777-7777-777777777777"),
                Title = "Blazor: Full-Stack Web Development",
                Description = "Create interactive web applications using Blazor Server and WebAssembly with C# instead of JavaScript.",
                PublicationDate = new DateTime(2024, 7, 25, 15, 45, 0, DateTimeKind.Utc)
            },
            new Course
            {
                Id = new Guid("88888888-8888-8888-8888-888888888888"),
                Title = "Azure Cloud Development with .NET",
                Description = "Deploy and scale .NET applications in Microsoft Azure with cloud-native development practices.",
                PublicationDate = new DateTime(2024, 8, 8, 12, 15, 0, DateTimeKind.Utc)
            },
            new Course
            {
                Id = new Guid("99999999-9999-9999-9999-999999999999"),
                Title = "Performance Optimization in .NET",
                Description = "Learn advanced techniques to optimize .NET application performance, memory management, and scalability.",
                PublicationDate = new DateTime(2024, 9, 14, 10, 30, 0, DateTimeKind.Utc)
            }
        };

        // Static pricing data with different tiers
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
            },
            new Price
            {
                Id = new Guid("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
                Name = "Premium Tier",
                CurrentPrice = 149.99m,
                PromotionalPrice = 119.99m
            },
            new Price
            {
                Id = new Guid("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
                Name = "Student Discount",
                CurrentPrice = 29.99m,
                PromotionalPrice = 19.99m
            },
            new Price
            {
                Id = new Guid("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"),
                Name = "Enterprise License",
                CurrentPrice = 299.99m,
                PromotionalPrice = 249.99m
            }
        };

        // Static instructor data with realistic professional profiles
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
            },
            new Instructor
            {
                Id = new Guid("23456789-2345-2345-2345-234567890123"),
                FirstName = "Michael",
                LastName = "Johnson",
                Degree = "Bachelor of Computer Engineering"
            },
            new Instructor
            {
                Id = new Guid("34567890-3456-3456-3456-345678901234"),
                FirstName = "Emily",
                LastName = "Davis",
                Degree = "Master of Information Technology"
            },
            new Instructor
            {
                Id = new Guid("45678901-4567-4567-4567-456789012345"),
                FirstName = "Robert",
                LastName = "Brown",
                Degree = "Certified Solutions Architect"
            },
            new Instructor
            {
                Id = new Guid("56789012-5678-5678-5678-567890123456"),
                FirstName = "Jennifer",
                LastName = "Miller",
                Degree = "Master of Business Administration"
            },
            new Instructor
            {
                Id = new Guid("67890123-6789-6789-6789-678901234567"),
                FirstName = "David",
                LastName = "Wilson",
                Degree = "PhD in Computer Science"
            },
            new Instructor
            {
                Id = new Guid("78901234-7890-7890-7890-789012345678"),
                FirstName = "Lisa",
                LastName = "Garcia",
                Degree = "Senior Software Developer"
            },
            new Instructor
            {
                Id = new Guid("89012345-8901-8901-8901-890123456789"),
                FirstName = "Christopher",
                LastName = "Martinez",
                Degree = "Cloud Solutions Expert"
            },
            new Instructor
            {
                Id = new Guid("90123456-9012-9012-9012-901234567890"),
                FirstName = "Amanda",
                LastName = "Taylor",
                Degree = "DevOps Engineering Specialist"
            }
        };

    return Tuple.Create(courses, prices, instructors);
    }
}