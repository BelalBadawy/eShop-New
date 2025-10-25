using eShop.Application.Features.Users.Commands;
using eShop.Application.Features.Users.Models.Requests;

namespace eShop.Application.Features.Users
{
    public interface IUserService
    {
        Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest userRegistration);
        Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest userUpdate);

        // Start
        Task<IResponseWrapper> GetUserByIdAsync(int userId);
        Task<IResponseWrapper> GetAllUsersAsync();
        Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest changePassword);
        Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest changeUserStatus);
        Task<IResponseWrapper> GetUserRolesAsync(string userId);
        Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRolesRequest updateUserRoles);
        Task<IResponseWrapper> GetUserByEmailAsync(string email);
        // End
    }
}
