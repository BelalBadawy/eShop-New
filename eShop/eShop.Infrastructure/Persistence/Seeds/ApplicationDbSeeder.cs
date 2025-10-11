using eShop.Infrastructure.Persistence.Contexts;

namespace eShop.Infrastructure.Persistence.DbInitializers
{
    public class ApplicationDbSeeder
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedApplicationDatabaseAsync()
        {
            await CheckAndApplyPendingMigrationAsync();
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
