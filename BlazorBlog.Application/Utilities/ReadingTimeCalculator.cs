namespace BlazorBlog.Application.Utilities
{
    using System.Text.RegularExpressions;

    public static class ReadingTimeCalculator
    {
        private const int AverageWordsPerMinute = 200;

        /// <summary>
        /// Calculates reading time and word count from HTML content
        /// </summary>
        /// <param name="html">HTML content to analyze</param>
        /// <returns>Tuple containing reading time string and word count</returns>
        public static (string readingTime, int wordCount) Calculate(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return ("0 min read", 0);
            }

            var text = Regex.Replace(html, "<[^>]+>", " ");
            
            text = Regex.Replace(text, @"\s+", " ").Trim();

            var words = Regex.Matches(text, @"\b[\p{L}\p{M}\w']+\b", RegexOptions.Multiline).Count;
            
            if (words == 0)
            {
                return ("0 min read", 0);
            }

            var minutes = Math.Max(1, (int)Math.Ceiling(words / (double)AverageWordsPerMinute));
            var readingTime = minutes == 1 ? "1 min read" : $"{minutes} min read";

            return (readingTime, words);
        }

        /// <summary>
        /// Calculates reading time from word count
        /// </summary>
        /// <param name="wordCount">Number of words</param>
        /// <returns>Reading time string</returns>
        public static string CalculateFromWordCount(int wordCount)
        {
            if (wordCount <= 0)
            {
                return "0 min read";
            }

            var minutes = Math.Max(1, (int)Math.Ceiling(wordCount / (double)AverageWordsPerMinute));
            return minutes == 1 ? "1 min read" : $"{minutes} min read";
        }
    }
}