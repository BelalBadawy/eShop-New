using eShop.Application.Features.Users.Models.Requests;

namespace eShop.Application.Features.Users.Commands
{
    public class UpdateUserRolesCommand : IRequest<IResponseWrapper>
    {
        public UpdateUserRolesRequest UpdateUserRoles { get; set; }
    }

    public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, IResponseWrapper>
    {
        private readonly IUserService _userService;

        public UpdateUserRolesCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUserRolesAsync(request.UpdateUserRoles);
        }
    }
}
