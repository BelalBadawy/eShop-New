namespace eShop.Application.Features.Roles
{
    public interface IRoleService
    {
        Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRole);
        Task<IResponseWrapper> GetRolesAsync();
        Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest updateRole);
        Task<IResponseWrapper> GetRoleByIdAsync(string roleId);
        Task<IResponseWrapper> DeleteRoleAsync(string roleId);
        Task<IResponseWrapper> GetPermissionsAsync(string roleId);
        Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRoleClaimsRequest updateRoleClaims);
    }
}
