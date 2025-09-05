namespace MasterNet.Application.Features.Account.Dtos;

/// <summary>
/// DTO para solicitar renovación de token
/// </summary>
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO para respuesta de tokens (login y refresh)
/// </summary>
public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}

/// <summary>
/// DTO para revocar tokens
/// </summary>
public class RevokeTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
