namespace BlazorBlog.Application.UI
{
    public interface IToastService
    {
        event Action<ToastLevel, string, string> OnShow;

        void ShowToast(ToastLevel level, string message, string heading = "");

        void RemoveAll();
    }
}
