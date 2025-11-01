using eShop.Application.Models.Pagination;

namespace eShop.Application.Features.Users.Queries
{
    public class GetUsersPagedQuery : IRequest<IResponseWrapper>, IValidateMe
    {
        public PagedFilterRequest PagedFilterRequest { get; set; }
    }

    public class GetUsersPagedQueryHandler : IRequestHandler<GetUsersPagedQuery, IResponseWrapper>
    {
        private readonly IUserService _userService;
        public GetUsersPagedQueryHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IResponseWrapper> Handle(GetUsersPagedQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetUsersPagedQueryAsync(request.PagedFilterRequest, cancellationToken);
        }
    }
}
