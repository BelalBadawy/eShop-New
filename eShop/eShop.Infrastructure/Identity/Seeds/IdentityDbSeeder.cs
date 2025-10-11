﻿using eShop.Infrastructure.Identity.Constants;
using eShop.Infrastructure.Identity.Models;
using eShop.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;

namespace eShop.Infrastructure.Persistence.DbInitializers
{
    public class IdentityDbSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public IdentityDbSeeder(ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedIdentityDatabaseAsync()
        {
            await CheckAndApplyPendingMigrationAsync();
            await SeedRolesAsync();
            await SeedAdminUserAsync();
            await SeedBasicUserAsync();
        }

        private async Task CheckAndApplyPendingMigrationAsync()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var user = new ApplicationUser
            {
                FullName = "Belal Badawy",
                Email = AppCredentials.Email,
                UserName = AppCredentials.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                PhoneNumber = "01025387387",
                NormalizedEmail = AppCredentials.Email.ToUpperInvariant(),
                NormalizedUserName = AppCredentials.Email.ToUpperInvariant(),
                IsActive = true,
                RefreshToken = "123",
                RefreshTokenExpiryDate = DateTime.Now.AddDays(1)
            };

            if (!await _userManager.Users.AnyAsync(u => u.Email == AppCredentials.Email))
            {
                var password = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = password.HashPassword(user, AppCredentials.Password);
                await _userManager.CreateAsync(user);
            }

            user = await _userManager.FindByEmailAsync(AppCredentials.Email);
            // Assign role(s) to user
            if (!await _userManager.IsInRoleAsync(user, AppRoles.Basic)
                && !await _userManager.IsInRoleAsync(user, AppRoles.Admin))
            {
                await _userManager.AddToRolesAsync(user, AppRoles.DefaultRoles);
            }
        }

        private async Task SeedBasicUserAsync()
        {
            var email = "asamy@gmail.com";
            var user = new ApplicationUser
            {
                FullName = "Ahmed Samy",
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                PhoneNumber = "0112929333",
                NormalizedEmail = email.ToUpperInvariant(),
                NormalizedUserName = email.ToUpperInvariant(),
                IsActive = true,
                RefreshToken = "321",
                RefreshTokenExpiryDate = DateTime.Now.AddDays(7)
            };

            if (!await _userManager.Users.AnyAsync(u => u.Email == email))
            {
                var password = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = password.HashPassword(user, AppCredentials.Password);
                await _userManager.CreateAsync(user);
            }

            user = await _userManager.FindByEmailAsync(email);

            // Assign role(s) to user
            if (!await _userManager.IsInRoleAsync(user, AppRoles.Basic))
            {
                await _userManager.AddToRoleAsync(user, AppRoles.Basic);
            }
        }

        private async Task SeedRolesAsync()
        {
            foreach (var roleName in AppRoles.DefaultRoles)
            {
                if (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName) is not ApplicationRole role)
                {
                    role = new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"{roleName} Role.",
                        NormalizedName = roleName.ToUpperInvariant()
                    };

                    await _roleManager.CreateAsync(role);
                }

                // Assign permissions to role
                if (roleName == AppRoles.Basic)
                {
                    await AssignPermissionsToRoleAsync(role, AppPermissions.BasicPermissions);
                }
                else if (roleName == AppRoles.Admin)
                {
                    await AssignPermissionsToRoleAsync(role, AppPermissions.AdminPermissions);
                }
            }
        }

        private async Task AssignPermissionsToRoleAsync(ApplicationRole role, IReadOnlyList<AppPermission> permmisions)
        {
            var currentlyAssignedClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var permission in permmisions)
            {
                if (!currentlyAssignedClaims.Any(claim => claim.Type == AppClaim.Permission && claim.Value == permission.Name))
                {
                    await _context.RoleClaims.AddAsync(new ApplicationRoleClaim
                    {
                        RoleId = role.Id,
                        ClaimType = AppClaim.Permission,
                        ClaimValue = permission.Name,
                        Description = permission.Description
                    });


                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
