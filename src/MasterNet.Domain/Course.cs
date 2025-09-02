namespace MasterNet.Domain;

/// <summary>
/// Entidad que representa un curso en el sistema.
/// Incluye información básica del curso y sus relaciones con otras entidades.
/// </summary>
public class Course : BaseEntity
{
    /// <summary>
    /// Título del curso. Es OBLIGATORIO por lógica de negocio.
    /// Se inicializa como non-nullable para reflejar que es requerido,
    /// pero permite asignación temporal de null durante construcción.
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// Descripción detallada del curso. Es OBLIGATORIA por lógica de negocio.
    /// Se inicializa como non-nullable para reflejar que es requerida.
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// Fecha de publicación del curso. Es requerida, pero nullable para permitir 
    /// cursos en estado borrador antes de ser publicados.
    /// </summary>
    public DateTime? PublicationDate { get; set; }

    // Propiedades de navegación - Siempre se inicializan para evitar null reference exceptions
    
    /// <summary>
    /// Colección de calificaciones asociadas al curso.
    /// Relación uno-a-muchos con Rating.
    /// </summary>
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    /// <summary>
    /// Colección de precios asociados al curso a través de la entidad intermedia.
    /// Relación muchos-a-muchos con Price.
    /// </summary>
    public ICollection<Price> Prices { get; set; } = new List<Price>();

    /// <summary>
    /// Colección de entidades intermedias para la relación muchos-a-muchos con Price.
    /// EF Core la necesita para manejar la relación.
    /// </summary>
    public ICollection<CoursePrice> CoursePrices { get; set; } = new List<CoursePrice>();

    /// <summary>
    /// Colección de instructores asociados al curso.
    /// Relación muchos-a-muchos con Instructor.
    /// </summary>
    public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

    /// <summary>
    /// Colección de entidades intermedias para la relación muchos-a-muchos con Instructor.
    /// EF Core la necesita para manejar la relación.
    /// </summary>
    public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();

    /// <summary>
    /// Colección de fotos asociadas al curso.
    /// Relación uno-a-muchos con Photo.
    /// </summary>
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}