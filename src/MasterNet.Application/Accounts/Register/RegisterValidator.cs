using FluentValidation;

namespace MasterNet.Application.Accounts.Register;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(2);
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3);
        // Degree is optional, so no validation needed
    }
}