using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MasterNet.Persistence.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Accounts.GetCurrentUser;

public class GetCurrentUserQuery
{
    public record GetCurrentUserQueryRequest(GetCurrentUserRequest getCurrentUserRequest)
        : IRequest<Result<Profile>>;

    internal class GetCurrentUserQueryHandler(
        UserManager<AppUser> userManager,
        ITokenService tokenService
        ) :
    IRequestHandler<GetCurrentUserQueryRequest, Result<Profile>>
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<Profile>> Handle(
            GetCurrentUserQueryRequest request, 
            CancellationToken cancellationToken
            )
        {
            var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Email == request.getCurrentUserRequest.Email, cancellationToken);

            if(user is null)
            {
                return Result<Profile>.Failure("User was not found");
            }

            var profile = new Profile
            {
                Email = user.Email,
                FullName = user.FullName,
                Token = await _tokenService.CreateToken(user),
                UserName = user.UserName
            };

            return Result<Profile>.Success(profile);
        }
    }
}