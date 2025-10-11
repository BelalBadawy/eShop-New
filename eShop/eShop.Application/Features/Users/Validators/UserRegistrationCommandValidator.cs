using IdentityService.Application.Features.Users.Commands;

namespace IdentityService.Application.Features.Users.Validators
{
    public class UserRegistrationCommandValidator : AbstractValidator<UserRegistrationCommand>
    {
        public UserRegistrationCommandValidator()
        {
            RuleFor(u => u.UserRegistration.FullName)
                .NotEmpty();
            RuleFor(u => u.UserRegistration.Email)
                .EmailAddress();
        }
    }
}
