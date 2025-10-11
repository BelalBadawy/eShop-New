
namespace eShop.Application.Interfaces
{
    public interface IPermissionChecker
    {
        bool HasClaim(string requiredClaim);
    }
}
