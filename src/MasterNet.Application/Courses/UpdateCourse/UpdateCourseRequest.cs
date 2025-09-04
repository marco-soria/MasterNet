using Microsoft.AspNetCore.Http;

namespace MasterNet.Application.Courses.UpdateCourse;

/// <summary>
/// Request para actualizar un curso existente.
/// Todas las propiedades son opcionales para permitir actualizaciones parciales.
/// </summary>
public class UpdateCourseRequest
{
    /// <summary>
    /// Título del curso - Opcional para actualizaciones parciales
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Descripción del curso - Opcional para actualizaciones parciales
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Fecha de publicación - Opcional para actualizaciones parciales
    /// </summary>
    public DateTime? PublicationDate { get; set; }
    
    /// <summary>
    /// Nueva foto del curso - Opcional. Si se proporciona, reemplaza la foto actual
    /// </summary>
    public IFormFile? Photo { get; set; }
    
    /// <summary>
    /// IDs de instructores para actualizar - Opcional. Si se proporciona, reemplaza la lista actual
    /// </summary>
    public List<Guid>? InstructorIds { get; set; }
    
    /// <summary>
    /// IDs de precios para actualizar - Opcional. Si se proporciona, reemplaza la lista actual
    /// </summary>
    public List<Guid>? PriceIds { get; set; }
}