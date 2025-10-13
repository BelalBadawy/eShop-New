namespace eShop.Application.Interfaces
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal? User { get; }              // Expose full ClaimsPrincipal
        string Name { get; }
        int? GetUserId();
        string GetUserEmail();
        bool IsAuthenticated();
        IList<string> GetRoles();
        IList<Claim> GetClaims();
        bool HasRole(string roleName);
        bool HasClaim(string claimType, string value);
        void SetCurrentUser(ClaimsPrincipal principal);
    }
}
