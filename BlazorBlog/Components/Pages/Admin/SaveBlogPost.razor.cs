namespace BlazorBlog.Components.Pages.Admin
{
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.JSInterop;
    using Category = BlazorBlog.Domain.Entities.Category;
    using BlogPost = BlazorBlog.Domain.Entities.BlogPost;

    public partial class SaveBlogPost : IAsyncDisposable
    {
        private const int MaxFileLength = 10 * 1024 * 1024;

        private static readonly IReadOnlyDictionary<string, string> AllowedImageExtensions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".gif"] = "image/gif",
            [".webp"] = "image/webp"
        };

        private bool _isLoading = false;
        private string? _loadingText = null;
        private BlogPostVm _blogPostVm = new BlogPostVm();
        private EditContext _editContext = default!;
        private ValidationMessageStore? _messageStore;
        private Category[] _categories = [];
        private string? _content = default!;
        private string? _errorMessage = null;
        private IBrowserFile? _fileToUpload;
        private string? _imageUrl;
        private string PageTitle => Id is > 0 ? "Edit Blog Post" : "New Blog Post";
        private bool _isSaving;
        private readonly string _editorId = $"blog-post-editor-{Guid.NewGuid():N}";
        private readonly string _toolbarId = $"blog-post-toolbar-{Guid.NewGuid():N}";
        public bool IsSaving => _isSaving;
        public string TagsCsv { get; set; } = string.Empty;

        private readonly CancellationTokenSource _cts = new();

        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] IWebHostEnvironment WebHostEnvironment { get; set; } = default!;
        [Inject] IBlogPostAdminService BlogPostService { get; set; } = default!;
        [Inject] ICategoryService CategoryService { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] IToastService ToastService { get; set; } = default!;
        [Inject] IHtmlSanitizer HtmlSanitizer { get; set; } = default!;
        [Inject] IValidator<BlogPostVm> Validator { get; set; } = default!;
        [Inject] ITagService TagService { get; set; } = default!;
        [Inject] ILogger<SaveBlogPost> Logger { get; set; } = default!;
        [Inject] IJSRuntime JsRuntime { get; set; } = default!;

        [Parameter] public int? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            _loadingText = "Loading blog post...";
            _editContext = new EditContext(_blogPostVm);
            _messageStore = new ValidationMessageStore(_editContext);

            _categories = await CategoryService.GetCategoriesAsync(_cts.Token);

            if (Id.HasValue && Id > 0)
            {
                var blogPost = await BlogPostService.GetBlogPostByIdAsync(Id.Value, _cts.Token);
                if (blogPost is null)
                {
                    NavigationManager.NavigateTo("/admin/manage-blog-posts", replace: true);
                    return;
                }

                _blogPostVm = blogPost.Adapt<BlogPostVm>();
                _editContext = new EditContext(_blogPostVm);
                _messageStore = new ValidationMessageStore(_editContext);
                _imageUrl = blogPost.Image;
                _content = blogPost.Content;

                try
                {
                    var tags = await TagService.GetTagsForPostAsync(blogPost.Id, _cts.Token);
                    TagsCsv = string.Join(", ", tags);
                }
                catch { /* optional */ }
            }
            _isLoading = false;
            _loadingText = null;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync(
                    "blazorBlogQuill.initialize",
                    _editorId,
                    _toolbarId,
                    _content ?? string.Empty,
                    "Enter your blog post content here...");
            }
        }

        private async Task PreviewImageAsync(IBrowserFile file)
        {
            if (!TryGetSafeImageExtension(file, out var extension, out _))
            {
                ToastService.ShowToast(ToastLevel.Warning, "Only JPG, PNG, GIF, or WebP images are allowed.", heading: "Warning");
                return;
            }

            try
            {
                await using var imageStream = file.OpenReadStream(maxAllowedSize: MaxFileLength);
                using MemoryStream ms = new MemoryStream();
                await imageStream.CopyToAsync(ms, _cts.Token);
                _imageUrl = $"data:{AllowedImageExtensions[extension]};base64,{Convert.ToBase64String(ms.ToArray())}";
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to preview uploaded image {FileName}.", file.Name);
                ToastService.ShowToast(ToastLevel.Warning, "The selected file is not a valid image.", heading: "Warning");
            }
        }

        private async Task HandleFileUploadAsync(InputFileChangeEventArgs e)
        {
            if (!TryGetSafeImageExtension(e.File, out _, out var errorMessage))
            {
                ToastService.ShowToast(ToastLevel.Warning, errorMessage, heading: "Warning");
                return;
            }

            await PreviewImageAsync(e.File);
            _fileToUpload = e.File;
            _blogPostVm.Image = e.File.Name;
        }

        private async Task SubmitAsync()
        {
            _messageStore!.Clear();

            var plain = (await JsRuntime.InvokeAsync<string>("blazorBlogQuill.getText", _editorId))?.Trim();
            if (string.IsNullOrWhiteSpace(plain))
            {
                var fi = new FieldIdentifier(_blogPostVm, nameof(_blogPostVm.Content));
                _messageStore.Add(fi, "The content is required.");
                _editContext.NotifyValidationStateChanged();
                return;
            }

            var html = await JsRuntime.InvokeAsync<string>("blazorBlogQuill.getHtml", _editorId);
            _blogPostVm.Content = HtmlSanitizer.Sanitize(html);

            var validationResult = await Validator.ValidateAsync(_blogPostVm, _cts.Token);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    var fi = new FieldIdentifier(_blogPostVm, error.PropertyName);
                    _messageStore.Add(fi, error.ErrorMessage);
                }
                _editContext.NotifyValidationStateChanged();
                return;
            }

            _isSaving = true;
            _isLoading = true;
            _loadingText = "Saving blog post...";
            StateHasChanged();
            await SaveBlogPostAsync();
        }

        private async Task SaveBlogPostAsync()
        {
            try
            {

                string? imageUrlToDelete = null;

                if (_fileToUpload is not null)
                {
                    var uploadedFileUrl = await SaveFileAsync(_fileToUpload);
                    if (uploadedFileUrl is null) return;

                    if (_blogPostVm.Id > 0 && !string.IsNullOrWhiteSpace(_blogPostVm.Image))
                    {
                        imageUrlToDelete = _blogPostVm.Image;
                    }

                    _blogPostVm.Image = uploadedFileUrl;
                }

                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var userId = authState.User.GetUserId();

                var entity = _blogPostVm.Adapt<BlogPost>();
                var result = await BlogPostService.SaveBlogPostAsync(entity, userId, _cts.Token);

                if (result is null)
                {
                    ToastService.ShowToast(ToastLevel.Error, "Something went wrong while saving the blog post.", heading: "Error");
                    _isSaving = false;
                    return;
                }

                _fileToUpload = null;
                if (imageUrlToDelete is not null) DeleteExistingImage(imageUrlToDelete);

                try
                {
                    var tags = TagsCsv
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(t => t)
                        .ToArray();
                    await TagService.SetTagsForPostAsync(result.Id, tags, _cts.Token);
                }
                catch
                {
                    ToastService.ShowToast(ToastLevel.Warning, "Saved post, but failed to save tags.", heading: "Warning");
                }

                NavigationManager.NavigateTo("/admin/manage-blog-posts");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to save blog post {BlogPostId}.", _blogPostVm.Id);
                ToastService.ShowToast(ToastLevel.Error, "Something went wrong while saving the blog post.", heading: "Error");
                _isSaving = false;
            }
            finally
            {
                _isLoading = false;
                _loadingText = null;
            }
        }

        private async Task<string?> SaveFileAsync(IBrowserFile file)
        {
            var webRootPath = WebHostEnvironment.WebRootPath;
            var folderPath = Path.Combine(webRootPath, "images", "posts");
            Directory.CreateDirectory(folderPath);

            if (!TryGetSafeImageExtension(file, out var extension, out var errorMessage))
            {
                ToastService.ShowToast(ToastLevel.Warning, errorMessage, heading: "Warning");
                return null;
            }

            var randomFileName = Path.GetRandomFileName();
            var filePath = Path.Combine(folderPath, randomFileName + extension);
            var fullFolderPath = EnsureTrailingSeparator(Path.GetFullPath(folderPath));
            var fullFilePath = Path.GetFullPath(filePath);

            if (!fullFilePath.StartsWith(fullFolderPath, StringComparison.OrdinalIgnoreCase))
            {
                ToastService.ShowToast(ToastLevel.Error, "Invalid upload path.", heading: "Error");
                return null;
            }

            try
            {
                await using FileStream fs = new(fullFilePath, FileMode.CreateNew);
                await file.OpenReadStream(maxAllowedSize: MaxFileLength).CopyToAsync(fs, _cts.Token);
                return Path.Combine("images", "posts", randomFileName + extension).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to save uploaded file {FileName}.", file.Name);
                ToastService.ShowToast(ToastLevel.Error, "Something went wrong while saving the file.", heading: "Error");
                return null;
            }
        }

        private void DeleteExistingImage(string imageUrl)
        {
            var postsFolder = EnsureTrailingSeparator(Path.GetFullPath(Path.Combine(WebHostEnvironment.WebRootPath, "images", "posts")));
            var imageToDelete = Path.Combine(WebHostEnvironment.WebRootPath, imageUrl.Replace("/", "\\"));
            var fullPath = Path.GetFullPath(imageToDelete);
            try
            {
                if (!fullPath.StartsWith(postsFolder, StringComparison.OrdinalIgnoreCase))
                {
                    Logger.LogWarning("Skipped deleting image outside the posts folder: {ImagePath}", imageUrl);
                    return;
                }

                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to delete existing blog post image {ImagePath}.", imageUrl);
            }
        }

        private static bool TryGetSafeImageExtension(IBrowserFile file, out string extension, out string errorMessage)
        {
            extension = Path.GetExtension(file.Name);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedImageExtensions.TryGetValue(extension, out var expectedContentType))
            {
                errorMessage = "Only JPG, PNG, GIF, or WebP images are allowed.";
                return false;
            }

            if (!string.Equals(file.ContentType, expectedContentType, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "The selected image type does not match the file extension.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private static string EnsureTrailingSeparator(string path)
        {
            return path.EndsWith(Path.DirectorySeparatorChar)
                ? path
                : path + Path.DirectorySeparatorChar;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await JsRuntime.InvokeVoidAsync("blazorBlogQuill.dispose", _editorId);
            }
            catch (JSDisconnectedException)
            {
            }

            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
