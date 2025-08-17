namespace BlazorBlog.Tests.Utilities
{
    using BlazorBlog.Application.Utilities;
    using Xunit;

    public class ReadingTimeCalculatorTests
    {
        [Theory]
        [InlineData("", "0 min read", 0)]
        [InlineData("<p>Hello world</p>", "1 min read", 2)]
        public void Calculate_FromHtml_Produces_Time_And_Count(string html, string expectedTime, int expectedCount)
        {
            var (time, count) = ReadingTimeCalculator.Calculate(html);
            Assert.Equal(expectedTime, time);
            Assert.Equal(expectedCount, count);
        }

        [Theory]
        [InlineData(0, "0 min read")]
        [InlineData(1, "1 min read")]
        [InlineData(400, "2 min read")]
        public void CalculateFromWordCount_Works(int words, string expected)
        {
            var time = ReadingTimeCalculator.CalculateFromWordCount(words);
            Assert.Equal(expected, time);
        }
    }
}
