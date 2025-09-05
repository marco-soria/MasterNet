using FluentValidation;
using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MasterNet.Domain.Security;
using MasterNet.Persistence.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Accounts.Register;

public class RegisterCommand
{

    public record RegisterCommandRequest(RegisterRequest registerRequest)
    : IRequest<Result<Profile>>;


    internal class RegisterCommandHandler(
        UserManager<AppUser> userManager,
        ITokenService tokenService
        )
        : IRequestHandler<RegisterCommandRequest, Result<Profile>>
    {
        
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<Profile>> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
           
            if(await  _userManager.Users
            .AnyAsync(x=> x.Email == request.registerRequest.Email))
            {
                return Result<Profile>.Failure("The email is already registered by another user");
            }

            if(await _userManager.Users
            .AnyAsync(x=>x.UserName == request.registerRequest.UserName))
            {
                return Result<Profile>.Failure("The username is already registered");
            }

             var user = new AppUser
             {
                FullName = request.registerRequest.FullName!,
                Id = Guid.NewGuid().ToString(),
                Degree = request.registerRequest.Degree,
                Email = request.registerRequest.Email,
                UserName  = request.registerRequest.UserName
             };
           
            var result =  await _userManager
            .CreateAsync(user, request.registerRequest.Password!);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, CustomRoles.CLIENT);
                
                var profile = new Profile
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    Token = await _tokenService.CreateToken(user),
                    UserName = user.UserName
                };

                return Result<Profile>.Success(profile);
            }

            return Result<Profile>.Failure("Errors occurred while registering the user");
        }
    }

    public class RegisterCommandRequestValidator : AbstractValidator<RegisterCommandRequest>
    {
        public RegisterCommandRequestValidator()
        {
            RuleFor(x => x.registerRequest).SetValidator(new RegisterValidator());
        }
    }


}