using eShop.Application.Features.Users;

namespace IdentityService.Application.Features.Users.Commands
{
    public class UserRegistrationCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public UserRegistrationRequest UserRegistration { get; set; }
    }

    public class UserRegistrationCommandHandler(IUserService userService)
        : IRequestHandler<UserRegistrationCommand, IResponseWrapper>
    {
        private readonly IUserService _userService = userService;

        public async Task<IResponseWrapper> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
        {
            return await _userService.RegisterUserAsync(request.UserRegistration);
        }
    }
}
