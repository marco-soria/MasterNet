using FluentValidation;
using MasterNet.Application.Core;
using MasterNet.Application.Features.Account.Dtos;
using MasterNet.Application.Interfaces;
using MasterNet.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MasterNet.Application.Features.Account.Commands;

/// <summary>
/// Comando para renovar access token usando refresh token
/// Optimizado para deployment en capas gratuitas con duraciones extendidas
/// </summary>
public class RefreshTokenCommand
{
    public record RefreshTokenCommandRequest(
        RefreshTokenRequestDto RefreshTokenRequest, 
        HttpContext? HttpContext = null
    ) : IRequest<Result<TokenResponseDto>>;

    internal class RefreshTokenCommandHandler(
        ITokenService tokenService
    ) : IRequestHandler<RefreshTokenCommandRequest, Result<TokenResponseDto>>
    {
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<TokenResponseDto>> Handle(
            RefreshTokenCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                // Obtener IP y User-Agent del contexto HTTP
                var ipAddress = request.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = request.HttpContext?.Request?.Headers["User-Agent"].ToString();

                // Renovar tokens
                var (accessToken, newRefreshToken) = await _tokenService.RefreshTokenAsync(
                    request.RefreshTokenRequest.RefreshToken,
                    ipAddress,
                    userAgent
                );

                var response = new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken.Token,
                    ExpiresAt = DateTime.UtcNow.AddDays(7), // 🚀 7 días para capas gratuitas
                    TokenType = "Bearer"
                };

                return Result<TokenResponseDto>.Success(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<TokenResponseDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<TokenResponseDto>.Failure($"Error refreshing token: {ex.Message}");
            }
        }
    }

    public class RefreshTokenCommandRequestValidator : AbstractValidator<RefreshTokenCommandRequest>
    {
        public RefreshTokenCommandRequestValidator()
        {
            RuleFor(x => x.RefreshTokenRequest.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required");
        }
    }
}
