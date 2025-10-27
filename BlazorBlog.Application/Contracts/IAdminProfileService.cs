namespace BlazorBlog.Application.Contracts
{
    using BlazorBlog.Application.ViewModels;

    public interface IAdminProfileService
    {
  Task<AdminProfileViewModel?> GetAdminProfileAsync(string userId);
        Task<bool> UpdateAdminProfileAsync(string userId, AdminProfileViewModel model);
        Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordViewModel model);
    }
}
