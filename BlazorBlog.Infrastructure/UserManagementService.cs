namespace BlazorBlog.Infrastructure
{
    using BlazorBlog.Application.Contracts;
    using BlazorBlog.Application.ViewModels;
    using BlazorBlog.Infrastructure.Persistence;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<ApplicationUser> _userStore;

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
        }

        public async Task<List<UserManagementViewModel>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);
            var userViewModels = new List<UserManagementViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email ?? string.Empty,
                    CurrentRole = roles.FirstOrDefault(),
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnd = user.LockoutEnd
                });
            }

            return userViewModels;
        }

        public async Task<List<string>> GetAvailableRolesAsync(CancellationToken cancellationToken = default)
        {
            var roles = await _roleManager.Roles
            .Select(r => r.Name!)
                  .ToListAsync(cancellationToken);

            return roles;
        }

        public async Task<(bool Success, string? Error)> AssignRoleAsync(
            string userId,
     string role,
       CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return (false, $"Role '{role}' does not exist");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                    return (false, $"Failed to remove existing roles: {errors}");
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, role);
            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                return (false, $"Failed to assign role: {errors}");
            }

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> RemoveRoleAsync(
           string userId,
               CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Any())
            {
                return (true, null);
            }

            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Failed to remove roles: {errors}");
            }

            return (true, null);
        }

        public async Task<string?> GetUserRoleAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }

        public async Task<(bool Success, string? Error)> LockUserAsync(
            string userId,
            DateTimeOffset lockoutEnd,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Failed to lock user: {errors}");
            }

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UnlockUserAsync(
    string userId,
         CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Failed to unlock user: {errors}");
            }

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> CreateUserAsync(
        CreateUserViewModel model,
              CancellationToken cancellationToken = default)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return (false, "A user with this email already exists");
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                return (false, $"Role '{model.Role}' does not exist");
            }

            var user = new ApplicationUser
            {
                Name = model.Name,
                Email = model.Email,
                EmailConfirmed = model.EmailConfirmed
            };

            await _userStore.SetUserNameAsync(user, model.Email, cancellationToken);

            if (_userStore is IUserEmailStore<ApplicationUser> emailStore)
            {
                await emailStore.SetEmailAsync(user, model.Email, cancellationToken);
            }

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return (false, $"Failed to create user: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return (false, $"User created but failed to assign role: {errors}");
            }

            return (true, null);
        }
    }
}