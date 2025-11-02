namespace BlazorBlog.Components.Pages.Admin;

using BlazorBlog.Application.ViewModels;

public partial class ManageUsers : IDisposable
{
    private List<UserManagementViewModel> _users = new();
    private List<string> _availableRoles = new();
    private bool _isLoading = true;
    private bool _isSaving = false;
    private string? _editingUserId;
    private string? _selectedRole;
    private readonly CancellationTokenSource _cts = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        _isLoading = true;
        try
        {
            _users = await UserManagementService.GetAllUsersAsync(_cts.Token);
            _availableRoles = await UserManagementService.GetAvailableRolesAsync(_cts.Token);
        }
        catch (Exception ex)
        {
            ToastService.ShowToast(ToastLevel.Error, $"Failed to load users: {ex.Message}", heading: "Error");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void EditRole(UserManagementViewModel user)
    {
        _editingUserId = user.Id;
        _selectedRole = user.CurrentRole;
    }

    private void CancelEdit()
    {
        _editingUserId = null;
        _selectedRole = null;
    }

    private async Task SaveRoleAsync(string userId)
    {
        if (_editingUserId != userId) return;

        _isSaving = true;
        try
        {
            (bool success, string? error) result;

            if (string.IsNullOrEmpty(_selectedRole))
            {
                // Remove role
                result = await UserManagementService.RemoveRoleAsync(userId, _cts.Token);
            }
            else
            {
                // Assign role
                result = await UserManagementService.AssignRoleAsync(userId, _selectedRole, _cts.Token);
            }

            if (result.success)
            {
                ToastService.ShowToast(ToastLevel.Success, "Role updated successfully", heading: "Success");
                await LoadDataAsync();
                CancelEdit();
            }
            else
            {
                ToastService.ShowToast(ToastLevel.Error, result.error ?? "Failed to update role", heading: "Error");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowToast(ToastLevel.Error, $"Error: {ex.Message}", heading: "Error");
        }
        finally
        {
            _isSaving = false;
        }
    }

    private async Task LockUserAsync(string userId)
    {
        try
        {
            var lockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // Effectively permanent
            var result = await UserManagementService.LockUserAsync(userId, lockoutEnd, _cts.Token);

            if (result.Success)
            {
                ToastService.ShowToast(ToastLevel.Success, "User locked successfully", heading: "Success");
                await LoadDataAsync();
            }
            else
            {
                ToastService.ShowToast(ToastLevel.Error, result.Error ?? "Failed to lock user", heading: "Error");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowToast(ToastLevel.Error, $"Error: {ex.Message}", heading: "Error");
        }
    }

    private async Task UnlockUserAsync(string userId)
    {
        try
        {
            var result = await UserManagementService.UnlockUserAsync(userId, _cts.Token);

            if (result.Success)
            {
                ToastService.ShowToast(ToastLevel.Success, "User unlocked successfully", heading: "Success");
                await LoadDataAsync();
            }
            else
            {
                ToastService.ShowToast(ToastLevel.Error, result.Error ?? "Failed to unlock user", heading: "Error");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowToast(ToastLevel.Error, $"Error: {ex.Message}", heading: "Error");
        }
    }

    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "?";
        if (parts.Length == 1) return parts[0][0].ToString().ToUpper();

        return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
    }

    private static string GetRoleBadgeClass(string role) => role switch
    {
        "Admin" => "bg-purple-50 text-purple-700 ring-1 ring-inset ring-purple-600/20 dark:bg-purple-600/15 dark:text-purple-300",
        "Editor" => "bg-green-50 text-green-700 ring-1 ring-inset ring-green-600/20 dark:bg-green-600/15 dark:text-green-300",
        _ => "bg-slate-50 text-slate-700 ring-1 ring-inset ring-slate-600/20 dark:bg-slate-600/15 dark:text-slate-300"
    };

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
