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

    /// <summary>
    /// 👤 Obtener perfil del usuario actual
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public ActionResult<Profile> GetCurrentUser()
    {
        var userName = _userAccessor.GetUserName();
        var userEmail = _userAccessor.GetUserEmail();
        
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized("User not found");
        }

        var profile = new Profile
        {
            Email = userEmail,
            UserName = userName
            // No incluimos tokens aquí por seguridad
        };

        return Ok(profile);
    }
}