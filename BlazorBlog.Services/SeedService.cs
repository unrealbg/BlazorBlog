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
            this._ctx = ctx;
            this._userStore = userStore;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._adminUserSettings = config.GetSection("AdminUser").Get<AdminUserSettings>();
        }

        public async Task SeedDataAsync()
        {
            await this.MigrateDatabaseAsync();

            if (await this._roleManager.FindByNameAsync(this._adminUserSettings.Role) is null)
            {
                var adminRole = new IdentityRole(this._adminUserSettings.Role);

                var result = await this._roleManager.CreateAsync(adminRole);
                if (!result.Succeeded)
                {
                    var errorsStr = result.Errors.Select(e => e.Description);
                    var errors = string.Join(Environment.NewLine, errorsStr);
                    throw new Exception($"Error creating admin role: {errors}");
                }
            }

            var adminUser = await this._userManager.FindByEmailAsync(this._adminUserSettings.Email);

            if (adminUser is null)
            {
                adminUser = new ApplicationUser();

                adminUser.Name = this._adminUserSettings.Name;
                adminUser.Email = this._adminUserSettings.Email;

                await this._userStore.SetUserNameAsync(adminUser, this._adminUserSettings.Email, CancellationToken.None);

                var emailStore = (IUserEmailStore<ApplicationUser>)this._userStore;
                await emailStore.SetEmailAsync(adminUser, this._adminUserSettings.Email, CancellationToken.None);

                var result = await this._userManager.CreateAsync(adminUser, this._adminUserSettings.Password);
                if (!result.Succeeded)
                {
                    var errorsStr = result.Errors.Select(e => e.Description);
                    var errors = string.Join(Environment.NewLine, errorsStr);
                    throw new Exception($"Error creating admin user: {errors}");
                }
            }

            if (!await this._ctx.Categories.AsNoTracking().AnyAsync())
            {
                await this._ctx.Categories.AddRangeAsync(Category.GetSeedCategories());
                await this._ctx.SaveChangesAsync();
            }
        }

        private async Task MigrateDatabaseAsync()
        {
#if DEBUG
            if (await this._ctx.Database.GetPendingMigrationsAsync() is { } migrations && migrations.Any())
            {
                await this._ctx.Database.MigrateAsync();
            }
#endif
        }
    }
}