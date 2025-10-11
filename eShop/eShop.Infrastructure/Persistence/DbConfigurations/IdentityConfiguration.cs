using eShop.Infrastructure.Identity.Models;
using eShop.Infrastructure.Persistence.Constants;
using Microsoft.AspNetCore.Identity;

namespace eShop.Infrastructure.Persistence.DbConfigurations
{
    internal class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users", SchemaNames.Identity);

            builder.Property(u => u.UserName)
                   .HasMaxLength(256);

            builder.Property(u => u.NormalizedUserName)
                   .HasMaxLength(256);

            builder.Property(u => u.Email)
                   .HasMaxLength(256);

            builder.Property(u => u.NormalizedEmail)
                   .HasMaxLength(256);

            builder.Property(u => u.PhoneNumber)
                   .HasMaxLength(20);

            builder.Property(u => u.ConcurrencyStamp)
                   .HasMaxLength(256);

            builder.Property(u => u.SecurityStamp)
                   .HasMaxLength(256);

            builder.Property(u => u.FullName)
                .HasMaxLength(256);

            builder.Property(u => u.RefreshToken)
                .HasMaxLength(500);


            // Example for custom field (if any)
            // builder.Property(u => u.FullName).HasMaxLength(200);
        }
    }

    internal class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.ToTable("Roles", SchemaNames.Identity);

            builder.Property(r => r.Name)
                   .HasMaxLength(100);

            builder.Property(r => r.NormalizedName)
                   .HasMaxLength(100);

            builder.Property(r => r.ConcurrencyStamp)
                   .HasMaxLength(256);
        }
    }

    internal class ApplicationRoleClaimConfig : IEntityTypeConfiguration<ApplicationRoleClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
        {
            builder.ToTable("RoleClaims", SchemaNames.Identity);

            builder.Property(rc => rc.ClaimType)
                   .HasMaxLength(256);

            builder.Property(rc => rc.ClaimValue)
                   .HasMaxLength(256);
        }
    }

    internal class IdentityUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<int>> builder)
        {
            builder.ToTable("UserRoles", SchemaNames.Identity);
        }
    }

    internal class IdentityUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<int>> builder)
        {
            builder.ToTable("UserClaims", SchemaNames.Identity);

            builder.Property(uc => uc.ClaimType)
                   .HasMaxLength(256);

            builder.Property(uc => uc.ClaimValue)
                   .HasMaxLength(256);
        }
    }

    internal class IdentityUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<int>> builder)
        {
            builder.ToTable("UserLogins", SchemaNames.Identity);

            builder.Property(l => l.LoginProvider)
                   .HasMaxLength(128);

            builder.Property(l => l.ProviderKey)
                   .HasMaxLength(128);

            builder.Property(l => l.ProviderDisplayName)
                   .HasMaxLength(256);
        }
    }

    internal class IdentityUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<int>> builder)
        {
            builder.ToTable("UserTokens", SchemaNames.Identity);

            builder.Property(t => t.LoginProvider)
                   .HasMaxLength(128);

            builder.Property(t => t.Name)
                   .HasMaxLength(128);

            builder.Property(t => t.Value)
                   .HasMaxLength(2048);
        }
    }
}
