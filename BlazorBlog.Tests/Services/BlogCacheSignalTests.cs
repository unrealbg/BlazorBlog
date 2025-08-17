namespace BlazorBlog.Tests.Services
{
    using BlazorBlog.Infrastructure.Utilities;
    using Xunit;

    public class BlogCacheSignalTests
    {
        [Fact]
        public void Bump_Increments_Version_Atomically()
        {
            var s = new BlogCacheSignal();
            var before = s.Version;
            s.Bump();
            s.Bump();
            Assert.True(s.Version > before);
        }
    }
}
