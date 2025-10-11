﻿using System.Collections.ObjectModel;

namespace eShop.Infrastructure.Identity.Constants
{
    internal class AppRoles
    {
        public const string Basic = nameof(Basic);
        public const string Admin = nameof(Admin);

        public static IReadOnlyList<string> DefaultRoles { get; }
            = new ReadOnlyCollection<string>(
            [
                Basic,
                Admin
            ]);
    }
}
