namespace MasterNet.Domain;

/// <summary>
/// Entidad que representa una foto asociada a un curso.
/// Almacena la URL de la imagen y su relación con el curso.
/// </summary>
public class Photo : BaseEntity
{
    /// <summary>
    /// URL de la foto. Es OBLIGATORIA por lógica de negocio.
    /// Se inicializa como non-nullable para reflejar que es requerida.
    /// </summary>
    public string Url { get; set; } = default!;

    public string? PublicId { get; set; } 

    /// <summary>
    /// Clave foránea hacia el curso al que pertenece la foto.
    /// Es requerida - una foto siempre debe estar asociada a un curso.
    /// Se mantiene nullable temporalmente para permitir validación a nivel de aplicación.
    /// </summary>
    public Guid? CourseId { get; set; }

    /// <summary>
    /// Propiedad de navegación hacia el curso propietario de la foto.
    /// Relación muchos-a-uno con Course.
    /// </summary>
    public Course? Course { get; set; }
}
