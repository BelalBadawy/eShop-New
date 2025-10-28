using eShop.Application.Features.Users;
using eShop.Application.Features.Users.Commands;
using eShop.Application.Features.Users.Models.Requests;
using eShop.Application.Features.Users.Queries;
using eShop.Application.Helpers;
using IdentityService.Application.Features.Users.Commands;

namespace WebApi.Controllers
{

    public class UsersController : BaseApiController
    {

        [HttpPost("register")]
        [MustHavePermission(AppService.Identity, AppFeature.Users, AppAction.Create)]
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserRegistrationRequest userRegistration)
        {
            var response = await Sender.Send(new UserRegistrationCommand { UserRegistration = userRegistration });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{userId:int}")]
        [MustHavePermission(AppService.Identity, AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetUserByIdAync(int userId)
        {
            var response = await Sender.Send(new GetUserByIdQuery { UserId = userId });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        [HttpGet("all")]
        [MustHavePermission(AppService.Identity, AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var response = await Sender.Send(new GetAllUsersQuery());

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut]
        [MustHavePermission(AppService.Identity, AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UpdateUserRequest updateUser)
        {
            var response = await Sender.Send(new UpdateUserCommand { UpdateUser = updateUser });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("change-password")]
        [MustHavePermission(AppService.Identity, AppFeature.Users, AppAction.Create)]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordRequest changePassword)
        {
            var response = await Sender.Send(new ChangeUserPasswordCommand { ChangePassword = changePassword });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("change-status")]
        [MustHavePermission(AppService.Identity, AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> ChangeUserStatus([FromBody] ChangeUserStatusRequest changeUserStatus)
        {
            var response = await Sender.Send(new ChangeUserStatusCommand { ChangeUserStatus = changeUserStatus });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("user-roles")]
        [MustHavePermission(AppService.Identity, AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesRequest updateUserRoles)
        {
            var response = await Sender
                .Send(new UpdateUserRolesCommand { UpdateUserRoles = updateUserRoles });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("roles/{userId}")]
        [MustHavePermission(AppService.Identity, AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetRoles(string userId)
        {
            var response = await Sender.Send(new GetUserRolesQuery { UserId = userId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
    }
}
