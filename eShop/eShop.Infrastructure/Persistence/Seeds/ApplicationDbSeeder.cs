using eShop.Infrastructure.Persistence.Contexts;

namespace eShop.Infrastructure.Persistence.DbInitializers
{
    public class ApplicationDbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IdentityDbSeeder identityDbSeeder;
        
        public ApplicationDbSeeder(ApplicationDbContext context,  IdentityDbSeeder identityDbSeeder)
        {
            _context = context;
            this.identityDbSeeder = identityDbSeeder;
        }

        public async Task SeedApplicationDatabaseAsync()
        {
            await CheckAndApplyPendingMigrationAsync();

            await identityDbSeeder.SeedIdentityDatabaseAsync();
        }

        private async Task CheckAndApplyPendingMigrationAsync()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }
        }

    }
}
