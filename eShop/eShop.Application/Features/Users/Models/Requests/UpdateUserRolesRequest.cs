using eShop.Application.Features.Users.Models.Responses;

namespace eShop.Application.Features.Users.Models.Requests
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public List<UserRoleViewModel> Roles { get; set; }
    }
}
