namespace BlazorBlog.Tests.Seo
{
    using System;
    using BlazorBlog.Seo;
    using Xunit;

    public class SeoEndpointExtensionsTests
    {
        [Fact]
        public void BuildSitemapCacheKey_ChangesWhenSignalVersionChanges()
        {
            var baseUri = new Uri("https://example.com/");

            var first = SeoEndpointExtensions.BuildSitemapCacheKey(baseUri, signalVersion: 1);
            var second = SeoEndpointExtensions.BuildSitemapCacheKey(baseUri, signalVersion: 2);

            Assert.NotEqual(first, second);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(15, 15)]
        [InlineData(2000, 1440)]
        public void Normalize_ClampsCacheMinutes(int configured, int expected)
        {
            var options = SeoEndpointExtensions.Normalize(new SitemapOptions { CacheMinutes = configured });

            Assert.Equal(expected, options.CacheMinutes);
        }
    }
}
