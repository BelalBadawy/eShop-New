using eShop.Application.Models.Pagination;

namespace eShop.Application.Features.Users.Queries
{
    public class GetUsersPagedQueryValidator : AbstractValidator<GetUsersPagedQuery>
    {
        public GetUsersPagedQueryValidator()
        {
            RuleFor(x => x.PagedFilterRequest)
                .SetValidator(new PagedFilterValidator());

            RuleFor(x => x.PagedFilterRequest.SortBy)
                .Must(field => new[] { "fullname", "email", "id" }.Contains(field))
                .WithMessage("Invalid SortBy value");
        }
    }
}
