namespace MasterNet.Domain.Security;

/// <summary>
/// Definición de Claims (Reclamos/Afirmaciones) personalizados para el sistema de autorización.
/// 
/// ¿Qué son los Claims?
/// - Son declaraciones sobre un usuario (ejemplo: "tiene permiso X", "es del departamento Y")
/// - Se almacenan en el token JWT o en la sesión del usuario
/// - Permiten autorización granular y flexible
/// 
/// ¿Por qué usar Claims?
/// - Más flexibles que roles simples
/// - Permiten autorizaciones complejas basadas en múltiples criterios
/// - Se pueden agregar/quitar dinámicamente sin cambiar código
/// 
/// Ejemplo de uso:
/// [Authorize(Policy = "RequiresPolicyClaim")]
/// public async Task<IActionResult> AdminAction() { ... }
/// </summary>
public static class CustomClaims
{
    /// <summary>
    /// Claim que indica que el usuario tiene políticas/permisos especiales asignados.
    /// Este claim se usa para verificar si el usuario tiene autorización para realizar
    /// acciones que requieren permisos específicos del sistema.
    /// 
    /// Uso típico:
    /// - Se asigna durante el login si el usuario tiene roles administrativos
    /// - Se verifica en controladores que requieren permisos especiales
    /// - Permite crear políticas de autorización basadas en este claim
    /// </summary>
    public const string POLICIES = nameof(POLICIES);

    // EJEMPLOS de otros claims que podrías agregar en el futuro:
    // public const string DEPARTMENT = nameof(DEPARTMENT);        // Departamento del usuario
    // public const string SUBSCRIPTION_LEVEL = nameof(SUBSCRIPTION_LEVEL); // Nivel de suscripción
    // public const string REGION = nameof(REGION);                // Región geográfica
    // public const string COURSE_ACCESS_LEVEL = nameof(COURSE_ACCESS_LEVEL); // Nivel de acceso a cursos
}
