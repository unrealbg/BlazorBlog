namespace BlazorBlog.Tests.Services
{
    using BlazorBlog.Infrastructure;
    using Xunit;

    public class SlugServiceTests
    {
        [Theory]
        [InlineData("Hello World", "hello-world")]
        [InlineData("  Hello—World!! ", "hello-world")]
        [InlineData("C#/.NET", "c-net")]
        public void GenerateSlug_NormalizesInput(string input, string expected)
        {
            var svc = new SlugService();
            var slug = svc.GenerateSlug(input);
            Assert.Equal(expected, slug);
        }

        [Fact]
        public void GenerateSlug_EmptyInput_ReturnsEmpty()
        {
            var svc = new SlugService();
            Assert.Equal(string.Empty, svc.GenerateSlug(" "));
        }
    }
}
