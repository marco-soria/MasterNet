namespace MasterNet.Domain.Security;

/// <summary>
/// Entidad que representa un refresh token para autenticación JWT.
/// Permite renovar tokens de acceso sin requerir nuevas credenciales.
/// Configurado para deployment en capas gratuitas con duraciones extendidas.
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// Token de renovación único y seguro (base64 encoded).
    /// </summary>
    public string Token { get; set; } = default!;

    /// <summary>
    /// Fecha de expiración del refresh token.
    /// Para deployment en capas gratuitas: 90 días por defecto.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Indica si el token ha sido revocado manualmente.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Fecha de creación del token.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha de revocación del token (si aplica).
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Razón de la revocación del token.
    /// </summary>
    public string? RevokedReason { get; set; }

    /// <summary>
    /// ID del usuario propietario del token.
    /// </summary>
    public string UserId { get; set; } = default!;

    /// <summary>
    /// Dirección IP desde donde se creó el token (para auditoría).
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User Agent del cliente que creó el token (para auditoría).
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Última fecha de uso del token (para métricas).
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Indica si el token está activo (no expirado ni revocado).
    /// </summary>
    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    /// <summary>
    /// Revoca el token con una razón específica.
    /// </summary>
    public void Revoke(string reason = "Manual revocation")
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
    }

    /// <summary>
    /// Actualiza la fecha de último uso del token.
    /// </summary>
    public void UpdateLastUsed()
    {
        LastUsedAt = DateTime.UtcNow;
    }
}
