using Microsoft.AspNetCore.Http;

namespace MasterNet.Application.Courses.CreateCourse;

/// <summary>
/// Request para crear un nuevo curso.
/// Propiedades requeridas alineadas con la entidad Course del dominio.
/// </summary>
public class CreateCourseRequest
{
    /// <summary>
    /// Título del curso - REQUERIDO según entidad Course
    /// </summary>
    public string Title { get; set; } = default!;
    
    /// <summary>
    /// Descripción del curso - REQUERIDA según entidad Course
    /// </summary>
    public string Description { get; set; } = default!;
    
    /// <summary>
    /// Fecha de publicación - Opcional, puede ser null para cursos en borrador
    /// </summary>
    public DateTime? PublicationDate { get; set; }
    
    /// <summary>
    /// Foto del curso - Opcional
    /// </summary>
    public IFormFile? Photo { get; set; }
    
    /// <summary>
    /// IDs de instructores a asociar al curso - Opcional pero recomendado
    /// </summary>
    public List<Guid>? InstructorIds { get; set; }
    
    /// <summary>
    /// IDs de precios a asociar al curso - Opcional pero recomendado
    /// </summary>
    public List<Guid>? PriceIds { get; set; }
}
