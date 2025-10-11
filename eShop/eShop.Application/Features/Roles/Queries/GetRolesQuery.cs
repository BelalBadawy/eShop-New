namespace eShop.Application.Features.Roles.Queries
{
    public class GetRolesQuery : IRequest<IResponseWrapper>
    {
    }

    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IResponseWrapper>
    {
        private readonly IRoleService _roleService;

        public GetRolesQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IResponseWrapper> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            return await _roleService.GetRolesAsync();
        }
    }
}
