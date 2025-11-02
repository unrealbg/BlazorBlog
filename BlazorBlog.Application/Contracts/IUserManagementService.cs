namespace BlazorBlog.Application.Contracts
{
    using BlazorBlog.Application.ViewModels;

    public interface IUserManagementService
    {
        /// <summary>
        /// Gets all users with their roles
        /// </summary>
        Task<List<UserManagementViewModel>> GetAllUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all available roles
        /// </summary>
        Task<List<string>> GetAvailableRolesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns a role to a user
        /// </summary>
        Task<(bool Success, string? Error)> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a user from all roles
        /// </summary>
        Task<(bool Success, string? Error)> RemoveRoleAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a user's current role
        /// </summary>
        Task<string?> GetUserRoleAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Locks out a user
        /// </summary>
        Task<(bool Success, string? Error)> LockUserAsync(string userId, DateTimeOffset lockoutEnd, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unlocks a user
        /// </summary>
        Task<(bool Success, string? Error)> UnlockUserAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new user with the specified role
        /// </summary>
        Task<(bool Success, string? Error)> CreateUserAsync(CreateUserViewModel model, CancellationToken cancellationToken = default);
    }
}