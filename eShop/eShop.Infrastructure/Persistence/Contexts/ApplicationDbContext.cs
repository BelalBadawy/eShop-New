using eShop.Application.Interfaces;
using eShop.Infrastructure.Identity.Models;
using eStoreCA.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace eShop.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser,      // TUser
        ApplicationRole,      // TRole
        int,                  // TKey
        ApplicationUserClaim, // TUserClaim
        ApplicationUserRole,  // TUserRole
        ApplicationUserLogin, // TUserLogin
        ApplicationRoleClaim, // TRoleClaim (This is where your class goes!)
        ApplicationUserToken> // TUserToken
    {
        private IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IConfiguration configuration,
            ICurrentUserService currentUserService)
            : base(options)
        {
            _configuration = configuration;
            _currentUserService = currentUserService;
        }

        //public DbSet<Brand> Brands => Set<Brand>();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            // optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // First: let IdentityDbContext configure all default Identity stuff
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Loop through all properties of type decimal

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {

                #region Decimal Value
                // Loop through all properties of type decimal
                var decimalProperties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

                foreach (var property in decimalProperties)
                {
                    // Example: decimal(18,6)
                    property.SetPrecision(18);
                    property.SetScale(6);
                }

                #endregion

                #region SoftDelete

                // Loop through all properties of type SoftDelete
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter();
                }

                #endregion

            }

            
        }
    }
}
