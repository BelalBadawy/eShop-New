namespace eShop.Application.Features.Users.Models.Responses
{
    public class UserRoleViewModel
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsAssignedToUser { get; set; }
    }
}
