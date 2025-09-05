using MasterNet.Domain.Security;
using Microsoft.AspNetCore.Identity;

namespace MasterNet.Persistence.Models;

/// <summary>
/// Entidad de usuario personalizada que extiende IdentityUser.
/// Representa un usuario del sistema con propiedades adicionales específicas del dominio.
/// 
/// ¿Por qué heredar de IdentityUser?
/// - IdentityUser ya incluye propiedades estándar: Id, UserName, Email, PasswordHash, etc.
/// - Nos permite agregar propiedades específicas de nuestro dominio de negocio
/// - Se integra automáticamente con el sistema de Identity de ASP.NET Core
/// </summary>
public class AppUser : IdentityUser
{
    /// <summary>
    /// Nombre completo del usuario. Es requerido por lógica de negocio.
    /// Se inicializa como non-nullable para reflejar que es obligatorio en el registro.
    /// Ejemplos: "Juan Carlos Pérez", "María González López"
    /// </summary>
    public string FullName { get; set; } = default!;

    /// <summary>
    /// Título académico o profesional del usuario. Es OPCIONAL.
    /// Puede ser null si el usuario no tiene un grado específico o no quiere especificarlo.
    /// Ejemplos: "Computer Science Degree", "Master in Software Engineering", null
    /// </summary>
    public string? Degree { get; set; }

    /// <summary>
    /// Colección de refresh tokens asociados al usuario.
    /// Permite manejar múltiples sesiones activas (diferentes dispositivos).
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    // NOTA: IdentityUser ya incluye estas propiedades heredadas:
    // - Id (string): Identificador único del usuario
    // - UserName (string): Nombre de usuario único
    // - Email (string): Correo electrónico 
    // - PasswordHash (string): Hash de la contraseña
    // - PhoneNumber (string?): Número de teléfono opcional
    // - EmailConfirmed (bool): Si el email está confirmado
    // - TwoFactorEnabled (bool): Si tiene 2FA habilitado
    // - LockoutEnd (DateTimeOffset?): Fecha de fin de bloqueo
    // - AccessFailedCount (int): Intentos fallidos de login
    // Y muchas más propiedades relacionadas con seguridad
}
