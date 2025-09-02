namespace MasterNet.Domain;

/// <summary>
/// Entidad que representa una calificación de un curso por parte de un estudiante.
/// Incluye puntuación numérica y comentario opcional.
/// </summary>
public class Rating : BaseEntity
{
    /// <summary>
    /// Nombre o identificador del estudiante que hace la calificación.
    /// Es OBLIGATORIO por lógica de negocio.
    /// </summary>
    public string Student { get; set; } = default!;

    /// <summary>
    /// Puntuación numérica del curso (ej: 1-5 estrellas).
    /// Es requerido y debe estar dentro del rango válido.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Comentario opcional del estudiante sobre el curso.
    /// Puede ser null si solo se proporciona puntuación.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Clave foránea hacia el curso calificado.
    /// Es requerida - una calificación siempre debe estar asociada a un curso.
    /// Se mantiene nullable temporalmente para permitir validación a nivel de aplicación.
    /// </summary>
    public Guid? CourseId { get; set; }

    /// <summary>
    /// Propiedad de navegación hacia el curso calificado.
    /// Relación muchos-a-uno con Course.
    /// </summary>
    public Course? Course { get; set; }
}
