namespace BlazorBlog.Tests.Feeds
{
    using System;
    using BlazorBlog.Feeds;
    using Xunit;

    public class FeedEndpointExtensionsTests
    {
        [Fact]
        public void BuildCacheKey_ChangesWhenSignalVersionChanges()
        {
            var baseUri = new Uri("https://example.com/");

            var first = FeedEndpointExtensions.BuildCacheKey(baseUri, itemCount: 20, signalVersion: 1);
            var second = FeedEndpointExtensions.BuildCacheKey(baseUri, itemCount: 20, signalVersion: 2);

            Assert.NotEqual(first, second);
        }

        [Fact]
        public void Normalize_ClampsConfigurableLimits()
        {
            var options = FeedEndpointExtensions.Normalize(new FeedOptions
            {
                Title = " ",
                Description = "",
                ItemCount = 500,
                CacheMinutes = 0
            });

            Assert.Equal("Blazor Blog", options.Title);
            Assert.Equal("Latest published posts from Blazor Blog.", options.Description);
            Assert.Equal(100, options.ItemCount);
            Assert.Equal(1, options.CacheMinutes);
        }
    }
}
