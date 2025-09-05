using FluentValidation;
using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MasterNet.Domain;
using MasterNet.Domain.Security;
using MasterNet.Persistence;
using MasterNet.Persistence.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Accounts.Login;

public class LoginCommand
{
    public record LoginCommandRequest(LoginRequest LoginRequest, HttpContext? HttpContext = null)
        : IRequest<Result<Profile>>, IBaseCommand;

    internal class LoginCommandHandler(
        UserManager<AppUser> userManager,
        ITokenService tokenService,
        MasterNetDbContext context
        )
        : IRequestHandler<LoginCommandRequest, Result<Profile>>
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly MasterNetDbContext _context = context;

        public async Task<Result<Profile>> Handle(
            LoginCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.Email == request.LoginRequest.Email);

            if (user is null)
            {
                return Result<Profile>.Failure("User was not found");
            }

            var result = await _userManager
                .CheckPasswordAsync(user, request.LoginRequest.Password!);

            if (!result)
            {
                return Result<Profile>.Failure("Invalid credentials");
            }

            // 🚀 Generar tokens para aplicación hobby con duraciones extendidas
            var accessToken = await _tokenService.CreateToken(user);
            
            // Obtener IP y User-Agent del contexto HTTP si está disponible
            var ipAddress = request.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = request.HttpContext?.Request?.Headers["User-Agent"].ToString();
            
            var refreshToken = _tokenService.GenerateRefreshToken(ipAddress, userAgent);
            refreshToken.UserId = user.Id;

            // Guardar refresh token en la base de datos
            await _context.Set<RefreshToken>().AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var profile = new Profile
            {
                Email = user.Email,
                FullName = user.FullName,
                UserName = user.UserName,
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                TokenExpiresAt = DateTime.UtcNow.AddDays(7) // 🚀 7 días para capas gratuitas
            };

            return Result<Profile>.Success(profile);
        }
    }

    public class LoginCommandRequestValidator : AbstractValidator<LoginCommandRequest>
    {
        public LoginCommandRequestValidator()
        {
            RuleFor(x => x.LoginRequest).SetValidator(new LoginValidator());
        }
    }
}