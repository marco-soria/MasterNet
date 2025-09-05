using System.Security.Claims;
using MasterNet.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MasterNet.Infrastructure.Security;

/// <summary>
/// Implementación concreta para acceder a información del usuario autenticado.
/// Extrae información de claims del HttpContext de manera segura.
/// </summary>
public class UserAccessor(IHttpContextAccessor httpContextAccessor) : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? GetUserName()
    {
        return _httpContextAccessor
            .HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
    }

    public string? GetUserId()
    {
        return _httpContextAccessor
            .HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public string? GetUserEmail()
    {
        return _httpContextAccessor
            .HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor
            .HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public IEnumerable<string> GetUserClaims(string claimType)
    {
        return _httpContextAccessor
            .HttpContext?.User?.FindAll(claimType)?.Select(c => c.Value) ?? Enumerable.Empty<string>();
    }
}