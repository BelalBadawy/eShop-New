
namespace eShop.Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public UpdateUserRequest UpdateUser { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResponseWrapper>
    {
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUserAsync(request.UpdateUser);
        }
    }
}
