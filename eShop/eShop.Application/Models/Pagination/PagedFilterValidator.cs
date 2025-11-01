namespace eShop.Application.Models.Pagination
{
    public class PagedFilterValidator : AbstractValidator<PagedFilterRequest>
    {
        public PagedFilterValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("PageNumber must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("PageSize must be greater than 0")
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize cannot exceed 100");

            RuleFor(x => x.SortDirection)
                .Must(x => x == "asc" || x == "desc")
                .WithMessage("SortDirection must be 'asc' or 'desc'");
        }
    }
}
