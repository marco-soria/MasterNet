using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MasterNet.Application.Interfaces;
using MasterNet.Domain;
using MasterNet.Domain.Security;
using MasterNet.Persistence;
using MasterNet.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MasterNet.Infrastructure.Security;

/// <summary>
/// Implementación del servicio de tokens JWT y refresh tokens.
/// Configurado para deployment en capas gratuitas con duraciones extendidas
/// para reducir la carga del servidor y mejorar la experiencia del usuario.
/// </summary>
public class TokenService(MasterNetDbContext context, IConfiguration configuration) : ITokenService
{
    private readonly MasterNetDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;

    // ⏰ DURACIONES OPTIMIZADAS PARA DEPLOYMENT EN CAPAS GRATUITAS
    // Access tokens largos para evitar reautenticación frecuente en servicios limitados
    private const int ACCESS_TOKEN_DAYS = 7;      // 7 días - reduce calls al servidor
    private const int REFRESH_TOKEN_DAYS = 90;    // 90 días - experiencia de usuario fluida

    public async Task<string> CreateToken(AppUser user)
    {
        var policies = await _context.Database.SqlQuery<string>($@"
                SELECT
                    aspr.ClaimValue
                FROM AspNetUsers a
                    LEFT JOIN AspNetUserRoles ar
                        ON a.Id=ar.UserId
                    LEFT JOIN AspNetRoleClaims aspr
                        ON ar.RoleId = aspr.RoleId
                    WHERE a.Id = {user.Id}
        ").ToListAsync();
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.GivenName, user.FullName) // ✅ Agregamos FullName a los claims
        };

        foreach(var policy in policies)
        {
            if(policy is not null)
            {
                claims.Add(new (CustomClaims.POLICIES, policy));
            }
        }

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"]!)),
            SecurityAlgorithms.HmacSha256
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(ACCESS_TOKEN_DAYS), // 🚀 7 días para capas gratuitas
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(string ipAddress, string? userAgent = null)
    {
        // Generar un token criptográficamente seguro
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var token = Convert.ToBase64String(randomBytes);

        return new RefreshToken
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(REFRESH_TOKEN_DAYS), // 🚀 90 días para capas gratuitas
            CreatedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };
    }

    public async Task<(string accessToken, RefreshToken refreshToken)> RefreshTokenAsync(
        string refreshToken, 
        string ipAddress, 
        string? userAgent = null)
    {
        var storedToken = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || !storedToken.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Actualizar último uso
        storedToken.UpdateLastUsed();

        // Obtener el usuario
        var user = await _context.Users.FindAsync(storedToken.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Generar nuevo access token
        var newAccessToken = await CreateToken(user);

        // Generar nuevo refresh token (rotación de tokens para seguridad)
        var newRefreshToken = GenerateRefreshToken(ipAddress, userAgent);
        newRefreshToken.UserId = user.Id;

        // Revocar el token anterior
        storedToken.Revoke("Replaced by new token");

        // Agregar el nuevo token
        await _context.Set<RefreshToken>().AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }

    public async Task<bool> RevokeTokenAsync(string token, string ipAddress, string reason = "Manual revocation")
    {
        var refreshToken = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken == null || !refreshToken.IsActive)
            return false;

        refreshToken.Revoke(reason);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RevokeAllUserTokensAsync(string userId, string reason = "Logout all sessions")
    {
        var userTokens = await _context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && rt.IsActive)
            .ToListAsync();

        if (!userTokens.Any())
            return false;

        foreach (var token in userTokens)
        {
            token.Revoke(reason);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}