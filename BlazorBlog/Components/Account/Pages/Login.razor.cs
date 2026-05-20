namespace BlazorBlog.Components.Account.Pages
{
    public partial class Login
    {
        private const string InvalidLoginMessage = "Error: Invalid login attempt.";

        private string? _errorMessage;
        private bool _isSubmitting;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm(Name = "Input")]
        private InputModel? FormInput { get; set; }

        private InputModel Input => FormInput ??= new();

        [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

        [Inject]
        SignInManager<BlazorBlog.Infrastructure.Persistence.ApplicationUser> SignInManager { get; set; } = default!;

        [Inject]
        ILogger<Login> Logger { get; set; } = default!;

        [Inject]
        NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        UserManager<BlazorBlog.Infrastructure.Persistence.ApplicationUser> UserManager { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (HttpMethods.IsGet(HttpContext.Request.Method))
            {
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        private async Task PerformLoginAsync()
        {
            _isSubmitting = true;
            try
            {
                var user = await UserManager.FindByEmailAsync(Input.Email);

                if (user is null)
                {
                    _errorMessage = InvalidLoginMessage;
                    return;
                }

                var result = await SignInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: true);

                if (result.IsLockedOut)
                {
                    Logger.LogWarning("User account locked out.");
                    _errorMessage = "Error: This account is locked. Please try again later.";
                    return;
                }

                if (!result.Succeeded)
                {
                    _errorMessage = InvalidLoginMessage;
                    return;
                }

                var additionalClaims = new[] { new System.Security.Claims.Claim(AppConstants.ClaimNames.FullName, user.Name) };

                await SignInManager.SignInWithClaimsAsync(user, Input.RememberMe, additionalClaims);

                Logger.LogInformation("User logged in.");
                HttpContext.Response.Redirect(GetSafeRedirectUrl());
            }
            finally
            {
                _isSubmitting = false;
            }
        }

        private string GetSafeRedirectUrl()
        {
            var redirectUrl = string.IsNullOrWhiteSpace(ReturnUrl) ? "/admin/dashboard" : ReturnUrl;

            if (redirectUrl.StartsWith("//", StringComparison.Ordinal) ||
                !Uri.IsWellFormedUriString(redirectUrl, UriKind.Relative))
            {
                return "/admin/dashboard";
            }

            return redirectUrl.StartsWith("/", StringComparison.Ordinal)
                ? redirectUrl
                : $"/{redirectUrl}";
        }
    }
}
