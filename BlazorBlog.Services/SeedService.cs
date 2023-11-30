﻿namespace BlazorBlog.Services
{
    using BlazorBlog.Data;
    using BlazorBlog.Data.Entities;
    using BlazorBlog.Services.Contracts;
    using BlazorBlog.Services.Settings;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class SeedService : ISeedService
    {
        private readonly AdminUserSettings adminUserSettings;

        private readonly ApplicationDbContext ctx;
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;


        public SeedService(ApplicationDbContext ctx, IUserStore<ApplicationUser> userStore,
                           UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
                           IConfiguration config)
        {
            this.ctx = ctx;
            this.userStore = userStore;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.adminUserSettings = config.GetSection("AdminUser").Get<AdminUserSettings>();
        }

        public async Task SeedDataAsync()
        {
            // Seed admin role from appsettings.json
            if (await this.roleManager.FindByNameAsync(this.adminUserSettings.Name) is null)
            {
                var adminRole = new IdentityRole(this.adminUserSettings.Role);

                var result = await this.roleManager.CreateAsync(adminRole);
                if (!result.Succeeded)
                {
                    var errorsStr = result.Errors.Select(e => e.Description);
                    var errors = string.Join(Environment.NewLine, errorsStr);
                    throw new Exception($"Error creating admin role: {errors}");
                }
            }

            var adminUser = await this.userManager.FindByEmailAsync(this.adminUserSettings.Email);

            if (adminUser is null)
            {
                adminUser = new ApplicationUser();

                adminUser.PasswordHash = this.userManager.PasswordHasher.HashPassword(adminUser, this.adminUserSettings.Password);
                adminUser.Name = this.adminUserSettings.Name;

                await this.userStore.SetUserNameAsync(adminUser, this.adminUserSettings.Email, CancellationToken.None);

                var result = await this.userManager.CreateAsync(adminUser, adminUser.PasswordHash);
                if (!result.Succeeded)
                {
                    var errorsStr = result.Errors.Select(e => e.Description);
                    var errors = string.Join(Environment.NewLine, errorsStr);
                    throw new Exception($"Error creating admin user: {errors}");
                }
            }

            if (!await this.ctx.Categories.AsNoTracking().AnyAsync())
            {
                await this.ctx.Categories.AddRangeAsync(Category.GetSeedCategories());
                await this.ctx.SaveChangesAsync();
            }
        }
    }
}