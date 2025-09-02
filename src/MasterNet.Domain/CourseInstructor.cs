namespace MasterNet.Domain;

/// <summary>
/// Entidad intermedia para la relación muchos-a-muchos entre Course e Instructor.
/// EF Core la utiliza para manejar la tabla de unión en la base de datos.
/// Las claves foráneas forman una clave primaria compuesta.
/// </summary>
public class CourseInstructor
{
    /// <summary>
    /// Clave foránea hacia el curso.
    /// Es parte de la clave primaria compuesta, por lo que es requerida.
    /// Se mantiene nullable temporalmente para permitir validación a nivel de aplicación.
    /// </summary>
    public Guid? CourseId { get; set; }

    /// <summary>
    /// Propiedad de navegación hacia el curso.
    /// Relación uno-a-muchos desde Course.
    /// </summary>
    public Course? Course { get; set; }

    /// <summary>
    /// Clave foránea hacia el instructor.
    /// Es parte de la clave primaria compuesta, por lo que es requerida.
    /// </summary>
    public Guid? InstructorId { get; set; }

    /// <summary>
    /// Propiedad de navegación hacia el instructor.
    /// Relación uno-a-muchos desde Instructor.
    /// </summary>
    public Instructor? Instructor { get; set; }
}
