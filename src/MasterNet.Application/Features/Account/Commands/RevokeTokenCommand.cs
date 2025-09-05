using FluentValidation;
using MasterNet.Application.Core;
using MasterNet.Application.Features.Account.Dtos;
using MasterNet.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MasterNet.Application.Features.Account.Commands;

/// <summary>
/// Comando para revocar refresh token específico
/// </summary>
public class RevokeTokenCommand
{
    public record RevokeTokenCommandRequest(
        RevokeTokenRequestDto RevokeTokenRequest, 
        HttpContext? HttpContext = null
    ) : IRequest<Result<bool>>;

    internal class RevokeTokenCommandHandler(
        ITokenService tokenService
    ) : IRequestHandler<RevokeTokenCommandRequest, Result<bool>>
    {
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<bool>> Handle(
            RevokeTokenCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var ipAddress = request.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
                var reason = $"Manual revocation from IP: {ipAddress}";

                var result = await _tokenService.RevokeTokenAsync(
                    request.RevokeTokenRequest.RefreshToken,
                    ipAddress,
                    reason
                );

                if (result)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Token not found or already revoked");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error revoking token: {ex.Message}");
            }
        }
    }

    public class RevokeTokenCommandRequestValidator : AbstractValidator<RevokeTokenCommandRequest>
    {
        public RevokeTokenCommandRequestValidator()
        {
            RuleFor(x => x.RevokeTokenRequest.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required");
        }
    }
}

/// <summary>
/// Comando para revocar todos los tokens de un usuario (logout de todas las sesiones)
/// </summary>
public class RevokeAllUserTokensCommand
{
    public record RevokeAllUserTokensCommandRequest(
        string UserId,
        HttpContext? HttpContext = null
    ) : IRequest<Result<bool>>;

    internal class RevokeAllUserTokensCommandHandler(
        ITokenService tokenService
    ) : IRequestHandler<RevokeAllUserTokensCommandRequest, Result<bool>>
    {
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<bool>> Handle(
            RevokeAllUserTokensCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var ipAddress = request.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
                var reason = $"Logout all sessions from IP: {ipAddress}";

                var result = await _tokenService.RevokeAllUserTokensAsync(
                    request.UserId,
                    reason
                );

                if (result)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("No active tokens found for user");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error revoking user tokens: {ex.Message}");
            }
        }
    }

    public class RevokeAllUserTokensCommandRequestValidator : AbstractValidator<RevokeAllUserTokensCommandRequest>
    {
        public RevokeAllUserTokensCommandRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");
        }
    }
}
