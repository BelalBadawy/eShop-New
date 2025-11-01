namespace eShop.Application.Features.Users.Queries
{
    public class GetUserByIdQuery : IRequest<IResponseWrapper>, IValidateMe
    {
        public int UserId { get; set; }
    }

    public class GetUserByIdQueryHanlder : IRequestHandler<GetUserByIdQuery, IResponseWrapper>
    {
        private readonly IUserService _userService;

        public GetUserByIdQueryHanlder(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetUserByIdAsync(request.UserId, cancellationToken);
        }
    }
}
