namespace BlazorBlog.Services
{
    public class SeedService : ISeedService
    {
        private readonly AdminUserSettings _adminUserSettings;

        private readonly ApplicationDbContext _ctx;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedService(ApplicationDbContext ctx, IUserStore<ApplicationUser> userStore,
                           UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
                           IConfiguration config)
        {
            _ctx = ctx;
            _userStore = userStore;
            _userManager = userManager;
            _roleManager = roleManager;
            _adminUserSettings = config.GetSection("AdminUser").Get<AdminUserSettings>();
        }

        public async Task SeedDataAsync()
        {
            await MigrateDatabaseAsync();

            if (await _roleManager.FindByNameAsync(_adminUserSettings.Role) is null)
            {
                var adminRole = new IdentityRole(_adminUserSettings.Role);

                var result = await _roleManager.CreateAsync(adminRole);
                if (!result.Succeeded)
                {
                    var errorsStr = result.Errors.Select(e => e.Description);
                    var errors = string.Join(Environment.NewLine, errorsStr);
                    throw new Exception($"Error creating admin role: {errors}");
                }
            }

            var adminUser = await _userManager.FindByEmailAsync(_adminUserSettings.Email);

            if (adminUser is null)
            {
                adminUser = new ApplicationUser();

                adminUser.Name = _adminUserSettings.Name;
                adminUser.Email = _adminUserSettings.Email;

                await _userStore.SetUserNameAsync(adminUser, _adminUserSettings.Email, CancellationToken.None);

                var emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
                await emailStore.SetEmailAsync(adminUser, _adminUserSettings.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(adminUser, _adminUserSettings.Password);
                if (!result.Succeeded)
                {
                    var errorsStr = result.Errors.Select(e => e.Description);
                    var errors = string.Join(Environment.NewLine, errorsStr);
                    throw new Exception($"Error creating admin user: {errors}");
                }
            }

            if (!await _ctx.Categories.AsNoTracking().AnyAsync())
            {
                await _ctx.Categories.AddRangeAsync(Category.GetSeedCategories());
                await _ctx.SaveChangesAsync();
            }
        }

        private async Task MigrateDatabaseAsync()
        {
#if DEBUG
            if (await _ctx.Database.GetPendingMigrationsAsync() is { } migrations && migrations.Any())
            {
                await _ctx.Database.MigrateAsync();
            }
#endif
        }
    }
}