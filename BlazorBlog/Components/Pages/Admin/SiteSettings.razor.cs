namespace BlazorBlog.Components.Pages.Admin
{
    public partial class SiteSettings
    {
        private SiteSettingsVm _settings = SiteSettingsVm.CreateDefault();
        private bool _isLoading = true;
        private bool _isSaving;

        [Inject]
        private ISiteSettingsService SiteSettingsService { get; set; } = default!;

        [Inject]
        private IToastService ToastService { get; set; } = default!;

        [Inject]
        private ILogger<SiteSettings> Logger { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _settings = await SiteSettingsService.GetSiteSettingsAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load site settings.");
                ToastService.ShowToast(ToastLevel.Error, "Failed to load site settings.", heading: "Error");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task SaveAsync()
        {
            _isSaving = true;

            try
            {
                _settings = await SiteSettingsService.SaveSiteSettingsAsync(_settings);
                ToastService.ShowToast(ToastLevel.Success, "Site settings saved.", heading: "Success");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to save site settings.");
                ToastService.ShowToast(ToastLevel.Error, "Failed to save site settings.", heading: "Error");
            }
            finally
            {
                _isSaving = false;
            }
        }

        private void ShowInvalidSettingsToast()
        {
            ToastService.ShowToast(ToastLevel.Warning, "Please fix the highlighted fields.", heading: "Validation");
        }
    }
}
