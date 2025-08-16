namespace BlazorBlog.Components.Account.Pages
{
    public partial class Login
    {
        private string? _errorMessage;
    private bool _showPassword;
    private bool _isSubmitting;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [SupplyParameterFromQuery] private string? ReturnUrl { get; set; } = "/admin/dashboard";

    [Inject] 
    SignInManager<BlazorBlog.Infrastructure.Persistence.ApplicationUser> SignInManager { get; set; } = default!;

    [Inject] 
    ILogger<Login> Logger { get; set; } = default!;

    [Inject] 
    NavigationManager NavigationManager { get; set; } = default!;

    [Inject] 
    IdentityRedirectManager RedirectManager { get; set; } = default!;

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
                    _errorMessage = "Error: Invalid login attempt.";
                    return;
                }

                var result = await SignInManager.CheckPasswordSignInAsync(user, Input.Password, false);

                if (!result.Succeeded)
                {
                    _errorMessage = "Error: Incorrect Password!";
                    return;
                }

                var additionalClaims = new[] { new System.Security.Claims.Claim(AppConstants.ClaimNames.FullName, user.Name) };

                await SignInManager.SignInWithClaimsAsync(user, Input.RememberMe, additionalClaims);

                Logger.LogInformation("User logged in.");
                RedirectManager.RedirectTo(ReturnUrl);
            }
            finally
            {
                _isSubmitting = false;
            }
        }

        private void ToggleShowPassword()
        {
            _showPassword = !_showPassword;
        }
    }
}
