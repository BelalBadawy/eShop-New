using eShop.Infrastructure.Identity.Constants;
using Microsoft.AspNetCore.Authorization;

namespace eShop.Infrastructure.Identity.Attributes
{
    public class MustHavePermissionAttribute : AuthorizeAttribute
    {
        public MustHavePermissionAttribute(string service, string feature, string action)
        {
            Policy = AppPermission.NameFor(service, feature, action);
        }
    }
}
