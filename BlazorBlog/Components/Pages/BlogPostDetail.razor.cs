namespace BlazorBlog.Components.Pages
{
    public partial class BlogPostDetail
    {
        private BlogPost _blogPost = new BlogPost();
        private BlogPost[] _relatedPosts = [];

        [Inject] 
        NavigationManager NavigationManager { get; set; }

        [Inject] 
        IBlogPostService BlogPostService { get; set; }

        [Parameter]
        public string BlogPostSlug { get; set; }

        private readonly CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            var result = await BlogPostService.GetBlogPostBySlugAsync(BlogPostSlug, _cts.Token);

            if (result.IsEmpty)
            {
                NavigationManager.NavigateTo("/", replace: true);
                return;
            }

            _blogPost = result.BlogPost;
            _relatedPosts = result.RelatedPosts;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
