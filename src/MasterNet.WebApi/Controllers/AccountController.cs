//using System.Net;
using MasterNet.Application.Accounts;
using MasterNet.Application.Accounts.Login;
using MasterNet.Application.Features.Account.Commands;
using MasterNet.Application.Features.Account.Dtos;
using MasterNet.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MasterNet.Application.Accounts.Login.LoginCommand;
using static MasterNet.Application.Features.Account.Commands.RefreshTokenCommand;
using static MasterNet.Application.Features.Account.Commands.RevokeTokenCommand;
using static MasterNet.Application.Features.Account.Commands.RevokeAllUserTokensCommand;
using MasterNet.Application.Accounts.Register;
using static MasterNet.Application.Accounts.Register.RegisterCommand;
using MasterNet.Application.Accounts.GetCurrentUser;
using static MasterNet.Application.Accounts.GetCurrentUser.GetCurrentUserQuery;

namespace MasterNet.WebApi.Controllers;

/// <summary>
/// Controlador de autenticación con soporte para refresh tokens
/// Optimizado para deployment en capas gratuitas con duraciones extendidas
/// </summary>
[ApiController]
[Route("api/account")]
public class AccountController(ISender sender, IUserAccessor userAccessor) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly IUserAccessor _userAccessor = userAccessor;

    /// <summary>
    /// 🚀 Login con generación de access token (7 días) y refresh token (90 días)
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<Profile>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginCommandRequest(request, HttpContext);
        var result = await _sender.Send(command, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : Unauthorized(result.Error);
    }

    /// <summary>
    /// 📝 Registro de nuevo usuario con rol CLIENT y token incluido
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<Profile>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new RegisterCommandRequest(request);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// 👤 Obtener perfil del usuario actual con información actualizada
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<Profile>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var email = _userAccessor.GetUserEmail();
        
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("User email not found in token");
        }
        
        var request = new GetCurrentUserRequest { Email = email };
        var query = new GetCurrentUserQueryRequest(request);
        var result = await _sender.Send(query, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return Unauthorized(result.Error);
        }

        // Por seguridad, no incluir token en respuesta de /me
        // Si necesitas token renovado, usa /refresh
        var profile = result.Value!;
        profile.Token = null;
        profile.RefreshToken = null;
        profile.TokenExpiresAt = null;

        return Ok(profile);
    }

    /// <summary>
    /// 🔄 Renovar access token usando refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var command = new RefreshTokenCommandRequest(request, HttpContext);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(result.Error);
    }

    /// <summary>
    /// 🚫 Revocar un refresh token específico
    /// </summary>
    [HttpPost("revoke")]
    public async Task<ActionResult> RevokeToken(
        [FromBody] RevokeTokenRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var command = new RevokeTokenCommandRequest(request, HttpContext);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(new { message = "Token revoked successfully" })
            : BadRequest(result.Error);
    }

    /// <summary>
    /// 🚪 Logout de todas las sesiones (revocar todos los refresh tokens del usuario)
    /// </summary>
    [Authorize]
    [HttpPost("logout-all")]
    public async Task<ActionResult> LogoutAllSessions(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token");
        }

        var command = new RevokeAllUserTokensCommandRequest(userId, HttpContext);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(new { message = "All sessions logged out successfully" })
            : BadRequest(result.Error);
    }
}