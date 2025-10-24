using eShop.Application.Features.Roles;
using eShop.Application.Features.Roles.Commands;
using eShop.Application.Features.Roles.Queries;

namespace WebApi.Controllers
{

    public class RolesController : BaseApiController
    {
        [HttpPost]
        [MustHavePermission(AppService.Identity, AppFeature.Roles, AppAction.Create)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var response = await Sender.Send(new CreateRoleCommand { CreateRole = request });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("all")]
        [MustHavePermission(AppService.Identity, AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var response = await Sender.Send(new GetRolesQuery());
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                return NotFound(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        [HttpPut]
        [MustHavePermission(AppService.Identity, AppFeature.Roles, AppAction.Update)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest updateRole)
        {
            var response = await Sender.Send(new UpdateRoleCommand { UpdateRole = updateRole });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{roleId}")]
        [MustHavePermission(AppService.Identity, AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            var response = await Sender.Send(new GetRoleByIdQuery { RoleId = roleId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpDelete("{roleId}")]
        [MustHavePermission(AppService.Identity, AppFeature.Roles, AppAction.Delete)]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var response = await Sender.Send(new DeleteRoleCommand { RoleId = roleId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("permissions/{roleId}")]
        [MustHavePermission(AppService.Identity, AppFeature.RoleClaims, AppAction.Read)]
        public async Task<IActionResult> GetPermissions(string roleId)
        {
            var response = await Sender.Send(new GetPermissionsQuery { RoleId = roleId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("update-permissions")]
        [MustHavePermission(AppService.Identity, AppFeature.RoleClaims, AppAction.Update)]
        public async Task<IActionResult> UpdateRolePermissions([FromBody] UpdateRoleClaimsRequest request)
        {
            var response = await Sender
                .Send(new UpdateRolePermissionsCommand { UpdateRoleClaims = request });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
