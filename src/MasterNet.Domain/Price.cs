namespace MasterNet.Domain;

/// <summary>
/// Entidad que representa un precio en el sistema.
/// Maneja tanto precios actuales como promocionales para los cursos.
/// </summary>
public class Price : BaseEntity
{
    /// <summary>
    /// Nombre descriptivo del precio (ej: "Precio Regular", "Precio Estudiante").
    /// Es OBLIGATORIO por lógica de negocio.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Precio actual del curso. Es requerido y no puede ser negativo.
    /// Tipo decimal para manejo preciso de moneda.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Precio promocional del curso. Es requerido.
    /// Puede ser igual al precio actual si no hay promoción.
    /// </summary>
    public decimal PromotionalPrice { get; set; }

    // Propiedades de navegación - Se inicializan para evitar null reference exceptions

    /// <summary>
    /// Colección de cursos asociados a este precio.
    /// Relación muchos-a-muchos con Course.
    /// </summary>
    public ICollection<Course> Courses { get; set; } = new List<Course>();

    /// <summary>
    /// Colección de entidades intermedias para la relación muchos-a-muchos con Course.
    /// EF Core la necesita para manejar la relación.
    /// </summary>
    public ICollection<CoursePrice> CoursePrices { get; set; } = new List<CoursePrice>();
}
