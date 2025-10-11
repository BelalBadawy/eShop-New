namespace eShop.Application.Features.Roles
{
    public class UpdateRoleClaimsRequest
    {
        public string RoleId { get; set; }
        public List<RoleClaimViewModel> RoleClaims { get; set; }
    }
}
