namespace BlazorBlog.Utilities
{
    using Ganss.Xss;
    using Markdig;

    public static class BlogContentRenderer
    {
        private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        public static string RenderSafeHtml(string content, IHtmlSanitizer sanitizer)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            var html = Markdown.ToHtml(content, Pipeline);
            return sanitizer.Sanitize(html);
        }
    }
}
