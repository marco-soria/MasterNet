namespace MasterNet.Application.Accounts;

/// <summary>
/// Perfil de usuario con tokens para deployment en capas gratuitas (duraciones extendidas)
/// </summary>
public class Profile
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? UserName { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
}