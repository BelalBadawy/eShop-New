using eShop.Application.Features.Roles;
using eShop.Application.Models;
using eShop.Infrastructure.Identity.Constants;
using eShop.Infrastructure.Identity.Models;
using eShop.Infrastructure.Persistence.Contexts;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace eShop.Infrastructure.Identity.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRole)
        {
            var roleInDb = await _roleManager.FindByNameAsync(createRole.Name);
            if (roleInDb is not null)
            {
                return await ResponseWrapper.FailAsync("Role already exists");
            }

            var newRole = new ApplicationRole
            {
                Name = createRole.Name,
                Description = createRole.Description
            };

            var identityResult = await _roleManager.CreateAsync(newRole);
            if (identityResult.Succeeded)
            {
                return await ResponseWrapper.SuccessAsync(message: "Role created successfully");
            }
            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
        }

        public async Task<IResponseWrapper> DeleteRoleAsync(string roleId)
        {
            var roleInDb = await _roleManager.FindByIdAsync(roleId);
            if (roleInDb is not null)
            {
                if (roleInDb.Name != AppRoles.Admin)
                {
                    var allUsers = await _userManager.Users.ToListAsync();
                    foreach (var user in allUsers)
                    {
                        if (await _userManager.IsInRoleAsync(user, roleInDb.Name))
                        {
                            return await ResponseWrapper
                                .FailAsync($"Role: {roleInDb.Name} is currently assigned to a user.");
                        }
                    }

                    var identityResult = await _roleManager.DeleteAsync(roleInDb);
                    if (identityResult.Succeeded)
                    {
                        return await ResponseWrapper.SuccessAsync("Role successfully deleted.");
                    }
                    return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
                }
                return await ResponseWrapper.FailAsync("Cannot delete Admin role.");
            }
            return await ResponseWrapper.FailAsync("Role does not exist.");
        }

        public async Task<IResponseWrapper> GetPermissionsAsync(string roleId)
        {
            var roleInDb = await _roleManager.FindByIdAsync(roleId);
            if (roleInDb is not null)
            {
                var allPermissions = AppPermissions.AllPermissions;
                var roleClaimResponse = new RoleClaimResponse
                {
                    Role = new RoleResponse
                    {
                        Id = roleId,
                        Name = roleInDb.Name,
                        Description = roleInDb.Description
                    },
                    RoleClaims = new List<RoleClaimViewModel>()
                };

                var currentlyAssignedClaims = await GetAllClaimsForRoleAsync(roleId);

                var allPermissionNames = allPermissions.Select(p => p.Name).ToList(); // Permission.Identity.Users.Create

                var currentlyAssignedClaimsValues = currentlyAssignedClaims
                    .Select(rc => rc.ClaimValue).ToList();// Permission.Identity.Users.Create

                var currentlyAssignedRoleClaimsNames = allPermissionNames
                    .Intersect(currentlyAssignedClaimsValues)
                    .ToList();

                foreach (var permission in allPermissions)
                {
                    if (currentlyAssignedRoleClaimsNames.Any(carc => carc == permission.Name))
                    {
                        roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                        {
                            RoleId = roleId,
                            ClaimType = AppClaim.Permission,
                            ClaimValue = permission.Name,
                            Description = permission.Description,
                            IsAssignedToRole = true
                        });
                    }
                    else
                    {
                        roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                        {
                            RoleId = roleId,
                            ClaimType = AppClaim.Permission,
                            ClaimValue = permission.Name,
                            Description = permission.Description,
                            IsAssignedToRole = false
                        });
                    }
                }
                return await ResponseWrapper<RoleClaimResponse>.SuccessAsync(data: roleClaimResponse);
            }
            return await ResponseWrapper.FailAsync(message: "Role does not exist.");
        }

        public async Task<IResponseWrapper> GetRoleByIdAsync(string roleId)
        {
            var roleInDb = await _roleManager.FindByIdAsync(roleId);
            if (roleInDb is not null)
            {
                var mappedRole = roleInDb.Adapt<RoleResponse>();
                return await ResponseWrapper<RoleResponse>.SuccessAsync(data: mappedRole);
            }
            return await ResponseWrapper.FailAsync("Role does not exist.");
        }

        public async Task<IResponseWrapper> GetRolesAsync()
        {
            var allRoles = await _roleManager.Roles.ToListAsync();
            if (allRoles.Count > 0)
            {
                var mappedRoles = allRoles.Adapt<List<RoleResponse>>();
                return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(data: mappedRoles);
            }
            return await ResponseWrapper.FailAsync("No roles were found.");
        }

        public async Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest updateRole)
        {
            var roleInDb = await _roleManager.FindByIdAsync(updateRole.RoleId);
            if (roleInDb is not null)
            {
                if (roleInDb.Name != AppRoles.Admin)
                {
                    roleInDb.Name = updateRole.Name;
                    roleInDb.Description = updateRole.Description;

                    var identityResult = await _roleManager.UpdateAsync(roleInDb);
                    if (identityResult.Succeeded)
                    {
                        return await ResponseWrapper.SuccessAsync("Role updated successfully");
                    }
                    return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));

                }
                return await ResponseWrapper.FailAsync("Cannot update Admin role.");
            }
            return await ResponseWrapper.FailAsync("Role does not exist.");
        }

        public async Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRoleClaimsRequest updateRoleClaims)
        {
            var roleInDb = await _roleManager.FindByIdAsync(updateRoleClaims.RoleId);
            if (roleInDb is not null)
            {
                if (roleInDb.Name == AppRoles.Admin)
                {
                    return await ResponseWrapper.FailAsync("Cannot change permissions for this role.");
                }
                var toBeAssignedPermissions = updateRoleClaims.RoleClaims
                    .Where(rc => rc.IsAssignedToRole == true)
                    .ToList();

                var currentlyAssignedPermissions = await _roleManager.GetClaimsAsync(roleInDb);

                // Dropping
                foreach (var claim in currentlyAssignedPermissions)
                {
                    await _roleManager.RemoveClaimAsync(roleInDb, claim);
                }

                // Lifting
                var mappedRoleClaims = toBeAssignedPermissions.Adapt<List<ApplicationRoleClaim>>();
                await _context.RoleClaims.AddRangeAsync(mappedRoleClaims);
                await _context.SaveChangesAsync();

                return await ResponseWrapper.SuccessAsync(message: "Role permissions updated successfully.");
            }
            return await ResponseWrapper.FailAsync(message: "Role does not exist.");
        }

        #region Private Helpers
        private List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
        {
            var errorDescriptions = new List<string>();
            foreach (var error in identityResult.Errors)
            {
                errorDescriptions.Add(error.Description);
            }
            return errorDescriptions;
        }

        private async Task<List<RoleClaimViewModel>> GetAllClaimsForRoleAsync(string roleId)
        {
            var roleClaims = await _context.RoleClaims
                .Where(rc => rc.RoleId == int.Parse(roleId))
                .ToListAsync();

            if (roleClaims.Count > 0)
            {
                var mappedRoleClaims = roleClaims.Adapt<List<RoleClaimViewModel>>();
                return mappedRoleClaims;
            }
            return [];
        }
        #endregion
    }
}
