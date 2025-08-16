namespace BlazorBlog.Infrastructure.Utilities
{
    public class BlogCacheSignal : IBlogCacheSignal
    {
        private long _version;

        public long Version => Interlocked.Read(ref _version);

        public void Bump() => Interlocked.Increment(ref _version);
    }
}
