namespace BlazorBlog.Infrastructure
{
    using BlazorBlog.Application.UI;

    public class ToastService : IToastService
    {
        public event Action<ToastLevel, string, string, int?>? OnShow;

        public void ShowToast(ToastLevel level, string message, string heading = "", int? durationMs = null)
        {
            OnShow?.Invoke(level, message, heading, durationMs);
        }

        public void RemoveAll()
        {
            // Use empty strings to avoid nulls
            OnShow?.Invoke(default, string.Empty, string.Empty, null);
        }
    }
}
