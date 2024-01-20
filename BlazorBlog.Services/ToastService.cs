namespace BlazorBlog.Services
{
    using Utilities;

    public class ToastService : IToastService
    {
        public event Action<ToastLevel, string, string> OnShow;
        public void ShowToast(ToastLevel level, string message, string heading = "")
        {
            OnShow?.Invoke(level, message, heading);
        }

        public void RemoveAll()
        {
            OnShow?.Invoke(default, null, null);
        }
    }
}
