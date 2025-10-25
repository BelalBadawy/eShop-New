using eShop.Application.Features.Users.Models.Requests;

namespace eShop.Application.Features.Users.Commands
{
    public class ChangeUserStatusCommand : IRequest<IResponseWrapper>
    {
        public ChangeUserStatusRequest ChangeUserStatus { get; set; }
    }

    public class ChangeUserStatusCommandHalder : IRequestHandler<ChangeUserStatusCommand, IResponseWrapper>
    {
        private readonly IUserService _userService;

        public ChangeUserStatusCommandHalder(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
        {
            return await _userService.ChangeUserStatusAsync(request.ChangeUserStatus);
        }
    }
}
