using eShop.Application.Models.JWT;
using eShop.Infrastructure.Identity.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace eShop.Infrastructure.Identity.Permissions
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IConfiguration _configuration;

        public PermissionAuthorizationHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User is null)
            {
                await Task.CompletedTask;
            }


            var jwtSettings = _configuration
                .GetSection("JwtConfiguration")
                .Get<JwtConfiguration>();

            var permissions = context.User.Claims
                .Where(claim => claim.Type == AppClaim.Permission
                    && claim.Value == requirement.Permission
                    && claim.Issuer == jwtSettings.Issuer);
            if (permissions.Any())
            {
                context.Succeed(requirement);
                await Task.CompletedTask;
            }
        }
    }
}
