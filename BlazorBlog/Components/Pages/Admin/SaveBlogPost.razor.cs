namespace BlazorBlog.Components.Pages.Admin
{
    using Microsoft.AspNetCore.Components.Forms;
    using Category = BlazorBlog.Domain.Entities.Category;
    using BlogPost = BlazorBlog.Domain.Entities.BlogPost;

    public partial class SaveBlogPost
    {
        private const int MaxFileLenght = 10 * 1024 * 1024;

        private bool _isLoading;
        private string? _loadingText;
        private BlogPostVm _blogPostVm = new BlogPostVm();
        private EditContext _editContext = default!;
        private ValidationMessageStore? _messageStore;
        private BlazoredTextEditor? _quillHtml;
        private Category[] _categories = [];
        private string? _content = default!;
    private string? _errorMessage = null;
        private IBrowserFile? _fileToUpload;
        private string? _imageUrl;
        private string PageTitle => Id is > 0 ? "Edit Blog Post" : "New Blog Post";

        private readonly CancellationTokenSource _cts = new();

    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] IWebHostEnvironment WebHostEnvironment { get; set; } = default!;
    [Inject] IBlogPostAdminService BlogPostService { get; set; } = default!;
    [Inject] ICategoryService CategoryService { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] BlazorBlog.Application.UI.IToastService ToastService { get; set; } = default!;
    [Inject] IHtmlSanitizer HtmlSanitizer { get; set; } = default!;
    [Inject] IValidator<BlogPostVm> Validator { get; set; } = default!;

        [Parameter] public int? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
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
            }
        }

        private async Task PreviewImageAsync(IBrowserFile file)
        {
            var extension = Path.GetExtension(file.Name)[1..];
            try
            {
                await using var imageStream = file.OpenReadStream(maxAllowedSize: MaxFileLenght);
                using MemoryStream ms = new MemoryStream();
                await imageStream.CopyToAsync(ms, _cts.Token);
                _imageUrl = $"data:image/{extension};base64,{Convert.ToBase64String(ms.ToArray())}";
            }
            catch
            {
                ToastService.ShowToast(BlazorBlog.Application.UI.ToastLevel.Warning, "The selected file is not a valid image.", heading: "Warning");
            }
        }

        private async Task HandleFileUploadAsync(InputFileChangeEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.File.ContentType) || !e.File.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                ToastService.ShowToast(BlazorBlog.Application.UI.ToastLevel.Warning, "Only image files are allowed.", heading: "Warning");
                return;
            }

            await PreviewImageAsync(e.File);
            _fileToUpload = e.File;
            _blogPostVm.Image = e.File.Name;
        }

        private async Task SubmitAsync()
        {
            _messageStore!.Clear();

            // Capture content from the editor BEFORE validation
            if (_quillHtml is not null)
            {
                var plain = (await _quillHtml.GetText())?.Trim();
                if (string.IsNullOrWhiteSpace(plain) || plain == "\n")
                {
                    var fi = new FieldIdentifier(_blogPostVm, nameof(_blogPostVm.Content));
                    _messageStore.Add(fi, "The content is required.");
                    _editContext.NotifyValidationStateChanged();
                    return;
                }

                var html = await _quillHtml.GetHTML();
                _blogPostVm.Content = HtmlSanitizer.Sanitize(html);
            }

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

            await SaveBlogPostAsync();
        }

        private async Task SaveBlogPostAsync()
        {
            try
            {
                _loadingText = "Saving blog post";
                _isLoading = true;

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
                    ToastService.ShowToast(BlazorBlog.Application.UI.ToastLevel.Error, "Something went wrong while saving the blog post.", heading: "Error");
                    _isLoading = false;
                    return;
                }

                _fileToUpload = null;
                if (imageUrlToDelete is not null) DeleteExistingImage(imageUrlToDelete);

                NavigationManager.NavigateTo("/admin/manage-blog-posts");
            }
            catch
            {
                ToastService.ShowToast(BlazorBlog.Application.UI.ToastLevel.Error, "Something went wrong while saving the blog post.", heading: "Error");
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
                await file.OpenReadStream(maxAllowedSize: MaxFileLenght).CopyToAsync(fs, _cts.Token);
                return Path.Combine("images", "posts", randomFileName + extension).Replace("\\", "/");
            }
            catch
            {
                ToastService.ShowToast(BlazorBlog.Application.UI.ToastLevel.Error, "Something went wrong while saving the file.", heading: "Error");
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
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch { }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
