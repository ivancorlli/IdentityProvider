using System.Security.Claims;
using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using IdentityProvider.Options;
using Microsoft.AspNetCore.Authentication;
using IdentityProvider.Interface;

namespace IdentityProvider.Pages
{
    public class SignInModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<ReturnUrlOptions> _defaultReturnUrl;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserLoginStore<ApplicationUser> _userLogin;
        private readonly ISmsSender _smsSender;
        public string? ReturnUrl { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public List<AuthenticationScheme> ExternalLogins { get; set; } = new();
        [BindProperty]
        public LoginModel Login { get; set; } = new LoginModel();

        public SignInModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signIn,
            IOptions<ReturnUrlOptions> retunrn,
            IUserStore<ApplicationUser> userStore,
            ISmsSender smsSender
            )
        {
            _userManager = userManager;
            _signIn = signIn;
            _defaultReturnUrl = retunrn;
            _userStore = userStore;
            _userLogin = GetLogins();
            _smsSender = smsSender;
        }

        public async Task<IActionResult?> OnGetAsync(string? returnUrl = null)
        {

            // If user is not comming for a oauth client we attach a default callback route
            if (string.IsNullOrEmpty(returnUrl))
            {
                ReturnUrl = _defaultReturnUrl.Value.Default.ToString();
            }
            else
            {
                ReturnUrl = returnUrl;
            }

            // Verify user is authenticated with the identity scheme
            var auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (auth.Succeeded)
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(id!.Value);
                if (user is not null)
                {
                    // If user exists, refresh his authentication cookie
                    await _signIn.RefreshSignInAsync(user);
                    // If phone is not verified or is it null redirect to page
                    if (user.PhoneNumber is null || user.PhoneNumberConfirmed is false)
                    {
                        return RedirectToPage("/ConfigPhone");
                    }
                    else
                    {
                        return new RedirectResult(ReturnUrl);
                    }
                }
                else
                {
                // Delete all their cookies
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                // Set all the external authentication methods
                var list = await _signIn.GetExternalAuthenticationSchemesAsync();
                ExternalLogins = list.ToList();
                return Page();
                }
            }
            else
            {
                // Delete all their cookies
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                // Set all the external authentication methods
                var list = await _signIn.GetExternalAuthenticationSchemesAsync();
                ExternalLogins = list.ToList();
                return Page();
            }
        }

        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                ReturnUrl = _defaultReturnUrl.Value.Default.ToString();
            }
            else
            {
                ReturnUrl = returnUrl;
            }
            // Sets all the external authentication methods
            var list = await _signIn.GetExternalAuthenticationSchemesAsync();
            ExternalLogins = list.ToList();
            // If the model is valid we can login the user
            if (ModelState.IsValid)
            {
                // search user
                ApplicationUser? user;
                if (Login.Email.Contains(char.Parse("@")))
                {
                    // get user by the email
                    var email = _userManager.NormalizeEmail(Login.Email);
                    user = await _userManager.FindByEmailAsync(email);
                }
                else
                {
                    // Get user by unsermae
                    user = await _userManager.FindByNameAsync(Login.Email);
                }

                // If the user exists we con follow with login
                if (user != null)
                {
                    // If the password is null, this meand the user has been authenticated with a social provider
                    if (string.IsNullOrEmpty(user.PasswordHash))
                    {
                        // search all the providers that the user has used to authenticate and return it in a informational message
                        var providers = await _userLogin.GetLoginsAsync(user, CancellationToken.None);
                        if (providers is not null)
                        {
                            if (providers.Count > 0)
                            {
                                var message = "";
                                if (providers.Count == 1)
                                {
                                    message = providers[0].ProviderDisplayName!.ToString();
                                }
                                else
                                {

                                    foreach (var provider in providers)
                                    {
                                        message += $"{provider.ProviderDisplayName}, ";
                                    }
                                }
                                Error = $"Puedes iniciar sesion con {message}";
                                return Page();
                            }
                            else
                            {
                                // If the user doesn't has any providers show an erro message
                                Error = "Usuario inexistente";
                                return Page();
                            }
                        }
                        else
                        {
                            // If the user doesn't has any providers show an erro message
                            Error = "Usuario inexistente";
                            return Page();
                        }
                    }

                    // If user exists we have to use the authentication method than the user prefer
                    if (
                        user.PhoneNumber is not null &&
                        user.PhoneNumberConfirmed &&
                        user.TwoFactorEnabled
                    )
                    {
                        // Check password is correct
                        // Create a two factor code
                        // Redirect to Two Factor Page
                        return RedirectToPage("/TwoFactor");
                    }
                    else
                    {
                        // Authenticate user with password
                        var result = await _signIn.PasswordSignInAsync(user, Login.Password, Login.Remember, lockoutOnFailure: false);
                        // If auth is succeed redirect user to corresponde page
                        if (result.Succeeded)
                        {
                            // If phone number is not set, redirect to page
                            if (user.PhoneNumber is null || user.PhoneNumberConfirmed is false)
                            {
                                return RedirectToPage("/ConfigPhone");
                            }else {
                                // otherwise redirect to callback url
                                return new RedirectResult(ReturnUrl);
                            }
                        }
                        // If user is not allowed it's means user's email hasn't been verified.
                        else if (result.IsNotAllowed)
                        {
                            // Show an erro messsage 
                            Error = "Debes verificar tu cuenta, por favor revisa tu correo electronico";
                            return Page();
                        }
                        else
                        {
                            // If till there is an error, probably the password is wrong
                            Error = "Contraseña incorrecta";
                            return Page();
                        }
                    }
                }
                else
                {
                    // If the user doesn't exists show an erro message
                    Error = "Usuario inexistente";
                    return Page();
                }
            }
            else
            {
                // When the model is invalid show respective error message
                foreach (var modelError in ModelState)
                {
                    if (modelError.Value.Errors.Count > 0)
                    {
                        Error = modelError.Value.Errors.First().ErrorMessage.ToString();
                        break;
                    }
                }
                return Page();
            }
        }


        public IActionResult OnPostToSignUp(string url)
        {
            return RedirectToPage("/Signup", new { ReturnUrl = url });
        }
        private IUserLoginStore<ApplicationUser> GetLogins()
        {
            return (IUserLoginStore<ApplicationUser>)_userStore;
        }
    }
}