using MasterNet.Domain.Security;
using MasterNet.Persistence.Models;

namespace MasterNet.Application.Interfaces;

/// <summary>
/// Servicio para manejo de tokens JWT y refresh tokens.
/// Abstrae la lógica de creación, validación y renovación de tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Crea un token JWT de acceso para el usuario.
    /// Duración para hobby: 24 horas (extendida para desarrollo).
    /// </summary>
    Task<string> CreateToken(AppUser user);

    /// <summary>
    /// Genera un refresh token seguro.
    /// Duración para hobby: 90 días (extendida para evitar relogins frecuentes).
    /// </summary>
    RefreshToken GenerateRefreshToken(string ipAddress, string? userAgent = null);

    /// <summary>
    /// Valida y renueva un token de acceso usando un refresh token.
    /// </summary>
    Task<(string accessToken, RefreshToken refreshToken)> RefreshTokenAsync(
        string refreshToken, 
        string ipAddress, 
        string? userAgent = null
    );

    /// <summary>
    /// Revoca un refresh token específico.
    /// </summary>
    Task<bool> RevokeTokenAsync(string token, string ipAddress, string reason = "Manual revocation");

    /// <summary>
    /// Revoca todos los refresh tokens de un usuario.
    /// Útil para logout de todas las sesiones.
    /// </summary>
    Task<bool> RevokeAllUserTokensAsync(string userId, string reason = "Logout all sessions");
}