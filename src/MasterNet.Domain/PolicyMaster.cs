namespace MasterNet.Domain;

/// <summary>
/// Definición de Políticas de autorización para el sistema.
/// 
/// ¿Qué son las Políticas (Policies)?
/// - Son reglas complejas de autorización que combinan roles, claims y lógica personalizada
/// - Permiten autorización más sofisticada que solo roles o claims
/// - Se definen en el startup/program.cs y se usan con [Authorize(Policy = "NombrePolicy")]
/// 
/// ¿Por qué usar Políticas?
/// - Centralizan la lógica de autorización
/// - Permiten combinar múltiples criterios (rol + claim + condición personalizada)
/// - Son reutilizables y mantenibles
/// - Facilitan testing de autorizaciones
/// 
/// Patrón de nomenclatura:
/// - [ENTIDAD]_[ACCIÓN]: Claro y consistente
/// - Usar MAYÚSCULAS para constantes
/// - Agrupar por funcionalidad
/// </summary>
public static class PolicyMaster
{
    #region Course Policies - Políticas para gestión de cursos
    
    /// <summary>
    /// Política para crear nuevos cursos.
    /// Típicamente requiere: Rol ADMIN o INSTRUCTOR + claim específico
    /// </summary>
    public const string COURSE_WRITE = nameof(COURSE_WRITE);
    
    /// <summary>
    /// Política para leer/ver cursos.
    /// Puede ser pública o requerir autenticación básica según el curso
    /// </summary>
    public const string COURSE_READ = nameof(COURSE_READ);
    
    /// <summary>
    /// Política para actualizar cursos existentes.
    /// Típicamente requiere: Ser creador del curso O tener rol ADMIN
    /// </summary>
    public const string COURSE_UPDATE = nameof(COURSE_UPDATE);
    
    /// <summary>
    /// Política para eliminar cursos.
    /// Típicamente requiere: Rol ADMIN + confirmación adicional
    /// </summary>
    public const string COURSE_DELETE = nameof(COURSE_DELETE);
    
    #endregion

    #region Instructor Policies - Políticas para gestión de instructores
    
    /// <summary>
    /// Política para ver información de instructores.
    /// Generalmente pública o con autenticación básica
    /// </summary>
    public const string INSTRUCTOR_READ = nameof(INSTRUCTOR_READ);
    
    /// <summary>
    /// Política para actualizar perfil de instructor.
    /// Requiere: Ser el mismo instructor O tener rol ADMIN
    /// </summary>
    public const string INSTRUCTOR_UPDATE = nameof(INSTRUCTOR_UPDATE);
    
    /// <summary>
    /// Política para crear nuevos perfiles de instructor.
    /// Requiere: Rol ADMIN + validaciones específicas
    /// </summary>
    public const string INSTRUCTOR_CREATE = nameof(INSTRUCTOR_CREATE);
    
    #endregion

    #region Comment Policies - Políticas para gestión de comentarios
    
    /// <summary>
    /// Política para leer comentarios.
    /// Generalmente pública para fomentar transparencia
    /// </summary>
    public const string COMMENT_READ = nameof(COMMENT_READ);
    
    /// <summary>
    /// Política para eliminar comentarios.
    /// Requiere: Ser autor del comentario O rol ADMIN/MODERATOR
    /// </summary>
    public const string COMMENT_DELETE = nameof(COMMENT_DELETE);
    
    /// <summary>
    /// Política para crear comentarios.
    /// Requiere: Usuario autenticado + haber comprado el curso (para ratings)
    /// </summary>
    public const string COMMENT_CREATE = nameof(COMMENT_CREATE);
    
    #endregion

    // EJEMPLOS de políticas adicionales que podrías necesitar:
    // #region User Management Policies
    // public const string USER_MANAGE = nameof(USER_MANAGE);      // Gestionar otros usuarios
    // public const string USER_VIEW_PROFILE = nameof(USER_VIEW_PROFILE); // Ver perfil de usuarios
    // #endregion
    
    // #region System Policies  
    // public const string SYSTEM_CONFIGURE = nameof(SYSTEM_CONFIGURE); // Configurar sistema
    // public const string REPORTS_VIEW = nameof(REPORTS_VIEW);      // Ver reportes
    // #endregion
}
