using eShop.Application.Features.Token.Queries;

namespace IdentityService.Application.Features.Users.Validators
{
    public class GetTokenQueryValidator : AbstractValidator<GetTokenQuery>
    {
        public GetTokenQueryValidator()
        {
            RuleFor(u => u.TokenRequest.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(u => u.TokenRequest.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}
