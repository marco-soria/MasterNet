using Bogus;
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
    /// Genera datos de prueba para poblar la base de datos durante el desarrollo.
    /// Utiliza la librería Bogus para generar datos ficticios realistas.
    /// </summary>
    /// <returns>Tupla con arrays de Course, Price e Instructor para usar como seed data</returns>
    public Tuple<Course[], Price[], Instructor[]> DataMaster()
    {
        var courses = new List<Course>();
        var faker = new Faker();

        // Genera 9 cursos con datos ficticios
        for (var i = 1; i < 10; i++)
        {
            var courseId = Guid.NewGuid();
            courses.Add(
                new Course
                {
                    Id = courseId,
                    Description = faker.Commerce.ProductDescription(),
                    Title = faker.Commerce.ProductName(),
                    PublicationDate = DateTime.UtcNow
                }
            );
        }

        // Crea un precio de ejemplo
        var priceId = Guid.NewGuid();
        var price = new Price
        {
            Id = priceId,
            CurrentPrice = 10.0m,
            PromotionalPrice = 8.0m,
            Name = "Regular Price"
        };
        var prices = new List<Price>
        {
            price
        };

        // Genera 10 instructores con datos ficticios usando Faker fluent API
        var fakerInstructor = new Faker<Instructor>()
            .RuleFor(t => t.Id, _ => Guid.NewGuid())
            .RuleFor(t => t.FirstName, f => f.Name.FirstName())
            .RuleFor(t => t.LastName, f => f.Name.LastName())
            .RuleFor(t => t.Degree, f => f.Name.JobTitle());

        var instructors = fakerInstructor.Generate(10);

        return Tuple.Create(courses.ToArray(), prices.ToArray(), instructors.ToArray());
    }
}