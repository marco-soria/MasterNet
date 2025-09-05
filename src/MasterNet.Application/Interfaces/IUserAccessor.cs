namespace MasterNet.Application.Interfaces;

/// <summary>
/// Interfaz para acceder a información del usuario autenticado.
/// Abstrae el acceso a claims del HttpContext.
/// </summary>
public interface IUserAccessor
{
    /// <summary>
    /// Obtiene el nombre de usuario del usuario autenticado.
    /// </summary>
    string? GetUserName();
    
    /// <summary>
    /// Obtiene el ID del usuario autenticado.
    /// </summary>
    string? GetUserId();
    
    /// <summary>
    /// Obtiene el email del usuario autenticado.
    /// </summary>
    string? GetUserEmail();
    
    /// <summary>
    /// Verifica si el usuario está autenticado.
    /// </summary>
    bool IsAuthenticated();
    
    /// <summary>
    /// Obtiene todos los claims del usuario autenticado.
    /// </summary>
    IEnumerable<string> GetUserClaims(string claimType);
}