namespace MasterNet.Domain;

/// <summary>
/// Entidad que representa un instructor en el sistema.
/// Contiene información personal y profesional del instructor.
/// </summary>
public class Instructor : BaseEntity
{
    /// <summary>
    /// Apellido del instructor. Es OBLIGATORIO por lógica de negocio.
    /// Se inicializa como non-nullable para reflejar que es requerido.
    /// </summary>
    public string LastName { get; set; } = default!;

    /// <summary>
    /// Nombre del instructor. Es OBLIGATORIO por lógica de negocio.
    /// Se inicializa como non-nullable para reflejar que es requerido.
    /// </summary>
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// Título académico o profesional del instructor.
    /// Puede ser null si no tiene un grado específico.
    /// </summary>
    public string? Degree { get; set; }

    // Propiedades de navegación - Se inicializan para evitar null reference exceptions

    /// <summary>
    /// Colección de cursos asociados al instructor.
    /// Relación muchos-a-muchos con Course.
    /// </summary>
    public ICollection<Course> Courses { get; set; } = new List<Course>();

    /// <summary>
    /// Colección de entidades intermedias para la relación muchos-a-muchos con Course.
    /// EF Core la necesita para manejar la relación.
    /// </summary>
    public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
}
