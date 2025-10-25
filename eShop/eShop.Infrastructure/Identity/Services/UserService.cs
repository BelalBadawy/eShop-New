using eShop.Application.Features.Users;
using eShop.Application.Features.Users.Commands;
using eShop.Application.Features.Users.Models.Requests;
using eShop.Application.Features.Users.Models.Responses;
using eShop.Application.Helpers;
using eShop.Application.Models;
using eShop.Infrastructure.Identity.Constants;
using eShop.Infrastructure.Identity.Models;
using IdentityService.Application.Features.Users;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace eShop.Infrastructure.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IdProtector _idProtector;

        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IdProtector idProtector)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _idProtector = idProtector;
        }

        public async Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest userRegistration)
        {
            var userWithSameEmail = await _userManager.FindByEmailAsync(userRegistration.Email);
            if (userWithSameEmail is not null)
                return await ResponseWrapper.FailAsync("Email address already taken.");

            var newUser = new ApplicationUser
            {
                FullName = userRegistration.FullName,
                Email = userRegistration.Email,
                UserName = userRegistration.Email,
                PhoneNumber = userRegistration.PhoneNumber,
                IsActive = userRegistration.ActivateUser,
                EmailConfirmed = userRegistration.AutoConfirmEmail,
                RefreshToken = DateTime.Now.Ticks.ToString(),
                RefreshTokenExpiryDate = DateTime.Now.AddDays(1)
            };

            // Password Hash
            var password = new PasswordHasher<ApplicationUser>();

            newUser.PasswordHash = password.HashPassword(newUser, userRegistration.Password);

            var identityUserResult = await _userManager.CreateAsync(newUser);

            if (identityUserResult.Succeeded)
            {
                var identityRoleResult = await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);

                if (identityRoleResult.Succeeded)
                {
                    return await ResponseWrapper.SuccessAsync("User registered successfully.");
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityRoleResult));
            }
            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityUserResult));
        }

        public async Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest userUpdate)
        {
            var userInDb = await _userManager.FindByIdAsync(userUpdate.UserId.ToString());
            if (userInDb is not null)
            {
                userInDb.FullName = userUpdate.FullName;
                userInDb.PhoneNumber = userUpdate.PhoneNumber;

                var identityResult = await _userManager.UpdateAsync(userInDb);
                if (identityResult.Succeeded)
                {
                    return await ResponseWrapper.SuccessAsync("User updated successfully.");
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
            }
            return await ResponseWrapper.FailAsync("User does not exists.");
        }

        #region Private Helpers
        private static List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
        {
            var errorDescriptions = new List<string>();
            foreach (var error in identityResult.Errors)
            {
                errorDescriptions.Add(error.Description);
            }
            return errorDescriptions;
        }
        #endregion

        public async Task<IResponseWrapper> GetUserByIdAsync(int userId)
        {
            var userInDb = await _userManager.FindByIdAsync(userId.ToString());
            if (userInDb is not null)
            {
                var mappedUser = userInDb.Adapt<UserResponse>();
                return await ResponseWrapper<UserResponse>.SuccessAsync(data: mappedUser);
            }
            return await ResponseWrapper.FailAsync("User does not exists.");
        }

        public async Task<IResponseWrapper> GetAllUsersAsync()
        {
            var usersInDb = await _userManager
                .Users
                .ToListAsync();

            if (usersInDb.Count > 0)
            {
                var mappedUsers = usersInDb.Adapt<List<UserResponse>>();
                foreach (var item in mappedUsers)
                {
                    item.Id = _idProtector.Protect(int.Parse(item.Id));
                }

                return await ResponseWrapper<List<UserResponse>>.SuccessAsync(data: mappedUsers);
            }
            return await ResponseWrapper.FailAsync("No Users were found.");
        }

        public async Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest changePassword)
        {
            var userInDb = await _userManager.FindByIdAsync(changePassword.UserId);
            if (userInDb is not null)
            {
                var identityResult = await _userManager.ChangePasswordAsync(
                userInDb,
                    changePassword.CurrentPassword,
                    changePassword.NewPassword);

                if (identityResult.Succeeded)
                {
                    return await ResponseWrapper.SuccessAsync(message: "User password updated.");
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
            }
            return await ResponseWrapper.FailAsync("User does not exist.");
        }

        public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest changeUserStatus)
        {
            var userInDb = await _userManager.FindByIdAsync(changeUserStatus.UserId);
            if (userInDb is not null)
            {
                // Change status
                userInDb.IsActive = changeUserStatus.ActivateOrDeactivate;

                var identityResult = await _userManager.UpdateAsync(userInDb);

                if (identityResult.Succeeded)
                {
                    return await ResponseWrapper
                        .SuccessAsync(changeUserStatus.ActivateOrDeactivate ? "User activated successfully."
                            : "User de-activated successfully");
                }
                return await ResponseWrapper
                    .FailAsync(GetIdentityResultErrorDescriptions(identityResult));
            }
            return await ResponseWrapper.FailAsync("User does not exist.");
        }

        public async Task<IResponseWrapper> GetUserRolesAsync(string userId)
        {
            var userRolesViewModel = new List<UserRoleViewModel>();
            var userInDb = await _userManager.FindByIdAsync(userId);

            if (userInDb is not null)
            {
                var allRoles = await _roleManager.Roles.ToListAsync();
                foreach (var role in allRoles)
                {
                    var userRoleViewModel = new UserRoleViewModel
                    {
                        RoleName = role.Name,
                        RoleDescription = role.Description
                    };

                    if (await _userManager.IsInRoleAsync(userInDb, role.Name))
                    {
                        userRoleViewModel.IsAssignedToUser = true;
                    }
                    else
                    {
                        userRoleViewModel.IsAssignedToUser = false;
                    }
                    userRolesViewModel.Add(userRoleViewModel);
                }

                return await ResponseWrapper<List<UserRoleViewModel>>.SuccessAsync(userRolesViewModel);
            }
            return await ResponseWrapper.FailAsync("User does not exist.");
        }

        public async Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRolesRequest updateUserRoles)
        {
            var userInDb = await _userManager.FindByIdAsync(updateUserRoles.UserId);
            if (userInDb is not null)
            {
                if (userInDb.Email == AppCredentials.Email)
                {
                    return await ResponseWrapper.FailAsync(message: "User Roles update not permitted.");
                }

                var currentAssigneRoles = await _userManager.GetRolesAsync(userInDb);

                var rolesToBeAssigned = updateUserRoles.Roles
                    .Where(role => role.IsAssignedToUser == true)
                    .ToList();

                var identityRemovingResult = await _userManager.RemoveFromRolesAsync(userInDb, currentAssigneRoles);

                if (identityRemovingResult.Succeeded)
                {
                    var identityAssigningResult = await _userManager
                        .AddToRolesAsync(userInDb, rolesToBeAssigned.Select(role => role.RoleName));

                    if (identityAssigningResult.Succeeded)
                    {
                        return await ResponseWrapper.SuccessAsync(message: "Updated user roles successfully.");
                    }
                    return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityAssigningResult));
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityRemovingResult));
            }
            return await ResponseWrapper.FailAsync("User does not exist.");
        }

        public async Task<IResponseWrapper> GetUserByEmailAsync(string email)
        {
            var userInDb = await _userManager.FindByEmailAsync(email);
            if (userInDb is not null)
            {
                var mappedUser = userInDb.Adapt<UserResponse>();
                return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
            }
            return await ResponseWrapper.FailAsync("User does not exist.");
        }
    }
}
