namespace BlazorBlog.Components.Pages.Admin
{

    using Ganss.Xss;
    using Blazored.TextEditor;

    public partial class SaveBlogPost
    {
        private bool _isLoading;
        private string? _loadingText;
        private BlogPost _blogPost = new BlogPost();
        private BlazoredTextEditor? _quillHtml;
        private Category[] _categories = [];
        private string? _errorMessage;
        private IBrowserFile? _fileToUpload;
        private string? _imageUrl;
        private string PageTitle => Id is > 0 ? "Edit Blog Post" : "New Blog Post";

        [Inject] 
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject] 
        IWebHostEnvironment WebHostEnvironment { get; set; }

        [Inject] 
        IBlogPostAdminService BlogPostService { get; set; }

        [Inject] 
        ICategoryService CategoryService { get; set; }

        [Inject] 
        IHtmlSanitizer HtmlSanitizer { get; set; }

        [Inject] 
        NavigationManager NavigationManager { get; set; }

        [Parameter]
        public int? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _categories = await CategoryService.GetCategoriesAsync();

            if (Id.HasValue && Id > 0)
            {
                var blogPost = await BlogPostService.GetBlogPostByIdAsync(Id.Value);

                if (blogPost is null)
                {
                    NavigationManager.NavigateTo("/admin/manage-blog-posts", replace: true);
                    return;
                }

                _blogPost = blogPost;
                _imageUrl = blogPost.Image;
            }
        }

        private async Task PreviewImageAsync(IBrowserFile file)
        {
            var extension = Path.GetExtension(file.Name)[1..];
            using var imageStream = file.OpenReadStream();
            using MemoryStream ms = new MemoryStream();
            await imageStream.CopyToAsync(ms);
            _imageUrl = $"data:image/{extension};base64,{Convert.ToBase64String(ms.ToArray())}";
        }

        private async Task HandleFileUploadAsync(InputFileChangeEventArgs e)
        {
            await PreviewImageAsync(e.File);
            _fileToUpload = e.File;
        }

        private async Task SaveBlogPostAsync()
        {
            try
            {
                var content = await _quillHtml!.GetHTML();

                if (string.IsNullOrEmpty(content))
                {
                    _errorMessage = "Blog post content is required.";
                    return;
                }

                _blogPost.Content = content;
                _loadingText = "Saving blog post";
                _isLoading = true;

                string? imageUrlToDelete = null;


                if (_fileToUpload is not null)
                {
                    var uploadedFileUrl = await SaveFileAsync(_fileToUpload);

                    if (uploadedFileUrl is null)
                    {
                        return;
                    }

                    if (_blogPost.Id > 0 && !string.IsNullOrWhiteSpace(_blogPost.Image))
                    {
                        imageUrlToDelete = _blogPost.Image;
                    }

                    _blogPost.Image = uploadedFileUrl;
                }

                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var userId = authState.User.GetUserId();

                await BlogPostService.SaveBlogPostAsync(_blogPost, userId);
                _fileToUpload = null;

                if (imageUrlToDelete is not null)
                {
                    DeleteExistingImage(imageUrlToDelete);
                }

                // _isLoading = false;
                NavigationManager.NavigateTo("/admin/manage-blog-posts");
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                _isLoading = false;
            }
        }

        private async Task<string?> SaveFileAsync(IBrowserFile file)
        {
            var webRootPath = WebHostEnvironment.WebRootPath;
            var folderPath = Path.Combine(webRootPath, "images", "posts");
            Directory.CreateDirectory(folderPath);

            var randomFileName = Path.GetRandomFileName();
            var extension = Path.GetExtension(file.Name);
            var filePath = Path.Combine(folderPath, randomFileName + extension);

            await using FileStream fs = new(filePath, FileMode.Create);

            try
            {
                await file.OpenReadStream().CopyToAsync(fs);
                return Path.Combine("images", "posts", randomFileName + extension).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                fs.Close();
                return null;
            }
        }

        private void DeleteExistingImage(string imageUrl)
        {

            var imageToDelete = Path.Combine(WebHostEnvironment.WebRootPath, imageUrl.Replace("/", "\\"));
            var fullPath = Path.GetFullPath(imageToDelete);

            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
