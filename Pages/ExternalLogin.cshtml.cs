using IdentityProvider.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages;

public class ExternalLogin : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;

    public string ReturnUrl {get;set;} = string.Empty;

    public ExternalLogin(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
    }







    public IActionResult OnGet() => RedirectToPage("./Login");

    public IActionResult OnPost(string provider, string? returnUrl = null)
    {
        // Request a redirect to the external login provider.
        var redirectUrl = Url.Page("/ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        if (remoteError != null)
        {
            // ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            // ErrorMessage = "Error loading external login information.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        // Sign in the user with this external login provider if the user already has a login.
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        if (result.IsLockedOut)
        {
            return RedirectToPage("./Lockout");
        }
        else
        {
            // If the user does not have an account, then ask the user to create an account.
            ReturnUrl = returnUrl;
            // ProviderDisplayName = info.ProviderDisplayName;
            // if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            // {
            //     Input = new InputModel
            //     {
            //         Email = info.Principal.FindFirstValue(ClaimTypes.Email)!
            //     };
            // }
            return Page();
        }
    }







        private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}