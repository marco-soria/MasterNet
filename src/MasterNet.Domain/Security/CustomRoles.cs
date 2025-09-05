namespace MasterNet.Domain.Security;

/// <summary>
/// Definición de Roles personalizados para el sistema de autorización.
/// 
/// ¿Qué son los Roles?
/// - Son categorías que agrupan usuarios con permisos similares
/// - Simplican la asignación de permisos masivos
/// - Son más fáciles de entender que claims individuales
/// 
/// ¿Cuándo usar Roles vs Claims?
/// - ROLES: Para categorías amplias de usuarios (Admin, Cliente, Moderador)
/// - CLAIMS: Para permisos específicos y granulares (CanDeleteComments, CanViewReports)
/// 
/// Patrón de nomenclatura:
/// - Usar MAYÚSCULAS para consistencia
/// - Nombres descriptivos y claros
/// - Evitar ambigüedades
/// </summary>
public static class CustomRoles
{
    /// <summary>
    /// Rol de Administrador del sistema.
    /// 
    /// Permisos típicos:
    /// - Gestión completa de cursos, instructores y usuarios
    /// - Acceso a reportes y estadísticas del sistema
    /// - Configuración del sistema y políticas de seguridad
    /// - Moderación de contenido y comentarios
    /// 
    /// Uso: usuarios internos, gerentes, administradores técnicos
    /// </summary>
    public const string ADMIN = nameof(ADMIN);

    /// <summary>
    /// Rol de Cliente/Usuario final del sistema.
    /// 
    /// Permisos típicos:
    /// - Ver y comprar cursos disponibles
    /// - Acceder a cursos comprados
    /// - Dejar comentarios y calificaciones
    /// - Gestionar su perfil personal
    /// 
    /// Uso: estudiantes, usuarios que consumen el contenido
    /// </summary>
    public const string CLIENT = nameof(CLIENT);

    // EJEMPLOS de otros roles que podrías agregar:
    // public const string INSTRUCTOR = nameof(INSTRUCTOR);    // Creadores de contenido
    // public const string MODERATOR = nameof(MODERATOR);      // Moderadores de contenido
    // public const string PREMIUM_CLIENT = nameof(PREMIUM_CLIENT); // Clientes con suscripción premium
    // public const string SUPPORT = nameof(SUPPORT);          // Equipo de soporte al cliente
}
