using eShop.Application.Features.Token.Queries;

namespace eShop.Application.Features.Users.Validators
{
    public class GetRefreshTokenQueryValidator : AbstractValidator<GetRefreshTokenQuery>
    {
        public GetRefreshTokenQueryValidator()
        {
            RuleFor(u => u.RefreshTokenRequest.Token)
                .NotEmpty();

            RuleFor(u => u.RefreshTokenRequest.RefreshToken)
                .NotEmpty();
        }
    }
}
