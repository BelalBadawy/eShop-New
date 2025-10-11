using System.Security.Claims;

namespace eShop.Application.Interfaces
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal? User { get; }              // Expose full ClaimsPrincipal

        int? UserId { get; }                        // Convenience property
        string? UserName { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }

        Task<IList<string>> GetRolesAsync();
        Task<IList<Claim>> GetClaimsAsync();

        bool HasRole(string roleName);
        bool HasClaim(string claimType, string value);
    }
}
