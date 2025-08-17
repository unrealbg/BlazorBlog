namespace BlazorBlog.Application.UI
{
    public interface IToastService
    {
    event Action<ToastLevel, string, string, int?> OnShow;

    void ShowToast(ToastLevel level, string message, string heading = "", int? durationMs = null);

        void RemoveAll();
    }
}
