namespace BlazorBlog.Infrastructure
{
    using BlazorBlog.Application.UI;

    public class ToastService : IToastService
    {
        public event Action<ToastLevel, string, string>? OnShow;

        public void ShowToast(ToastLevel level, string message, string heading = "")
        {
            OnShow?.Invoke(level, message, heading);
        }

        public void RemoveAll()
        {
            // Use empty strings to avoid nulls
            OnShow?.Invoke(default, string.Empty, string.Empty);
        }
    }
}
