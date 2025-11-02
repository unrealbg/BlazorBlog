namespace BlazorBlog.Components.Pages.Admin;

using BlazorBlog.Application.ViewModels;

public partial class CreateUser : IDisposable
{
    private CreateUserViewModel _model = new();
    private List<string> _availableRoles = new();
    private bool _isSubmitting = false;
    private bool _showPassword = false;
    private readonly CancellationTokenSource _cts = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _availableRoles = await UserManagementService.GetAvailableRolesAsync(_cts.Token);
        }
        catch (Exception ex)
        {
            ToastService.ShowToast(ToastLevel.Error, $"Failed to load roles: {ex.Message}", heading: "Error");
        }
    }

    private async Task CreateUserAsync()
    {
        _isSubmitting = true;
        try
        {
            var result = await UserManagementService.CreateUserAsync(_model, _cts.Token);

            if (result.Success)
            {
                ToastService.ShowToast(
                ToastLevel.Success,
               $"User '{_model.Name}' created successfully with {_model.Role} role",
                  heading: "User Created");

                Navigation.NavigateTo("/admin/manage-users");
            }
            else
            {
                ToastService.ShowToast(ToastLevel.Error, result.Error ?? "Failed to create user", heading: "Error");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowToast(ToastLevel.Error, $"Error: {ex.Message}", heading: "Error");
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
