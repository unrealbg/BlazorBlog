namespace BlazorBlog.Components.Account.Pages
{
    public partial class Login
    {
        private string? _errorMessage;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [SupplyParameterFromQuery] private string? ReturnUrl { get; set; } = "/admin/dashboard";

        [Inject] 
        SignInManager<ApplicationUser> SignInManager { get; set; }

        [Inject] 
        ILogger<Login> Logger { get; set; }

        [Inject] 
        NavigationManager NavigationManager { get; set; }

        [Inject] 
        IdentityRedirectManager RedirectManager { get; set; }

        [Inject]
        UserManager<ApplicationUser> UserManager { get; set; }

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

            Claim[] additionalClaims = [new Claim(AppConstants.ClaimNames.FullName, user.Name)];

            await SignInManager.SignInWithClaimsAsync(user, Input.RememberMe, additionalClaims);

            Logger.LogInformation("User logged in.");
            RedirectManager.RedirectTo(ReturnUrl);
        }
    }
}
