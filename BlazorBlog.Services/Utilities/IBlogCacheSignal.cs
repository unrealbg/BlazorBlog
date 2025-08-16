namespace BlazorBlog.Services.Utilities
{
    public interface IBlogCacheSignal
    {
        void Bump();

        long Version { get; }
    }
}