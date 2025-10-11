using System.Collections.ObjectModel;

namespace eShop.Infrastructure.Identity.Constants
{
    public record AppPermission(string Service, string Feature, string Action, string Description, bool IsBasic = false)
    {
        public string Name => NameFor(Service, Feature, Action);
        public static string NameFor(string service, string feature, string action)
        {
            return $"Permission.{service}.{feature}.{action}"; // Permission.Identity.Users.Create
        }
    }

    public class AppPermissions
    {
        private static readonly AppPermission[] _all =
        [
            new (AppService.Identity, AppFeature.Users, AppAction.Create,  "Create Users"),
            new (AppService.Identity, AppFeature.Users, AppAction.Read, "Read Users"),
            new (AppService.Identity, AppFeature.Users, AppAction.Update, "Update Users"),
            new (AppService.Identity, AppFeature.Users, AppAction.Delete, "Delete Users"),

            new (AppService.Identity, AppFeature.Roles, AppAction.Create, "Create Roles"),
            new (AppService.Identity, AppFeature.Roles, AppAction.Read, "Read Roles"),
            new (AppService.Identity, AppFeature.Roles, AppAction.Update, "Update Roles"),
            new (AppService.Identity, AppFeature.Roles, AppAction.Delete, "Delete Roles"),

            new (AppService.Identity, AppFeature.UserRoles, AppAction.Read, "Read User Roles"),
            new (AppService.Identity, AppFeature.UserRoles, AppAction.Update, "Update User Roles"),

            new (AppService.Identity, AppFeature.RoleClaims, AppAction.Read, "Read Role Claims/Permissions"),
            new (AppService.Identity, AppFeature.RoleClaims, AppAction.Update, "Update Role Claims/Permissions"),

            new (AppService.Product, AppFeature.Brands, AppAction.Create, "Create Brands"),
            new (AppService.Product, AppFeature.Brands, AppAction.Update, "Update Brands"),
            new (AppService.Product, AppFeature.Brands, AppAction.Read, "Read Brands", IsBasic: true),
            new (AppService.Product, AppFeature.Brands, AppAction.Delete, "Delete Brands"),

        ];

        public static IReadOnlyList<AppPermission> AllPermissions { get; } =
            new ReadOnlyCollection<AppPermission>(_all);
        public static IReadOnlyList<AppPermission> AdminPermissions { get; } =
            new ReadOnlyCollection<AppPermission>(_all.Where(p => !p.IsBasic).ToArray());
        public static IReadOnlyList<AppPermission> BasicPermissions { get; } =
            new ReadOnlyCollection<AppPermission>(_all.Where(p => p.IsBasic).ToArray());
    }
}
