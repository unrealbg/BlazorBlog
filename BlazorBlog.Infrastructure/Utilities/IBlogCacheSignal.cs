namespace BlazorBlog.Infrastructure.Utilities
{
    public interface IBlogCacheSignal
    {
        void Bump();

        long Version { get; }
    }
}