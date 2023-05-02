using IdentityProvider.Entity;
using IdentityProvider.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using IdentityProvider.Interface;
using IdentityProvider.Constant;
using IdentityProvider.Enumerables;
using IdentityProvider.ValueObject;

namespace IdentityProvider.Pages;

public class ExternalLogin : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserLoginStore<ApplicationUser> _userLogin;
    private readonly IOptions<ReturnUrlOptions> _defaultReturn;
    private readonly IEmailSender _emailSender;
    private readonly ISmsSender _smsSender;

    public string ReturnUrl { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public bool AllowBack { get; set; } = false;
    public ExternalLogin(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IUserStore<ApplicationUser> userStore,
        IOptions<ReturnUrlOptions> returnUrl,
        IEmailSender emailSender,
        ISmsSender smsSender
        )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _roleManager = roleManager;
        _defaultReturn = returnUrl;
        _emailSender = emailSender;
        _userLogin = GetLogins();
        _smsSender = smsSender;
    }

    public IActionResult OnGet() => RedirectToPage("/Signin");

    public IActionResult OnPost(string provider, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl) || string.IsNullOrEmpty(provider))
            return Redirect("/Signin");

        ReturnUrl = returnUrl;
        var redirectUrl = Url.Page("/ExternalLogin", pageHandler: "Callback", values: new { ReturnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        ReturnUrl = returnUrl ?? _defaultReturn.Value.Default;
        if (remoteError != null)
        {
            Error = $"Error del provedor: {remoteError}";
            return Page();
        }
        // Gets information about exteranl provider
        ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            // If tehre is not information, return an error
            Error = "Se produjo un error al obtener tus datos.";
            return Page();
        }
        else
        {
            // Get Email claim
            string email = info.Principal.FindFirstValue(ClaimTypes.Email)!;
            // Search if user is already registered with this provider
            ApplicationUser? user = await _userLogin.FindByLoginAsync(info.LoginProvider, info.ProviderKey, CancellationToken.None);

            // IF user is not registered
            if (user == null)
            {
                // Create a new external account
                user = ApplicationUser.CreateExternalUser(email);
                // Add Profile claims
                if (!string.IsNullOrEmpty(info.Principal.FindFirstValue(ClaimTypes.Name)))
                {
                    string name = info.Principal.FindFirstValue(ClaimTypes.GivenName)!;
                    string surname = info.Principal.FindFirstValue(ClaimTypes.Surname)!;
                    PersonName personName = new(name, surname);
                    user.CreateProfile(personName);
                    if (!string.IsNullOrEmpty(info.Principal.FindFirstValue("image")))
                    {
                        user.Profile!.AddProfileImage(info.Principal.FindFirstValue("Image")!);
                    }
                }
                // save in database
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    // Aggreagate a rol, bydeafult an application role
                    bool role = await _roleManager.RoleExistsAsync(DefaultRoles.ApplicationUser);
                    if (role) await _userManager.AddToRoleAsync(user, DefaultRoles.ApplicationUser);
                    else await _userManager.AddToRoleAsync(user, DefaultRoles.DefaultUser);


                    // save the provider
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        var Result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

                        if (Result.Succeeded)
                        {
                            await _emailSender.SendWelcome(email);
                            if (user.PhoneNumber is null || user.NormalizedUserName == user.NormalizedEmail) return RedirectToPage("/QuickStartProfile", new { ReturnUrl });
                            else if (user.PhoneNumber is not null && !user.PhoneNumberConfirmed) return RedirectToPage("/ConfirmPhone", new { ReturnUrl });
                            else if (user.PhoneTwoFactorEnabled) return RedirectToPage("/TwoFactor", new { ReturnUrl, Remember = false });
                            else return new RedirectResult(ReturnUrl);
                        }
                        else
                        {
                            Error = $"Se produjo un error al obtener tus datos de {info.ProviderDisplayName}.";
                            AllowBack = true;
                            return Page();
                        }
                    }
                    else
                    {
                        // if there is any error saving the provider
                        foreach (var error in result.Errors)
                        {
                            Error = error.Description;
                            break;
                        }
                        return Page();
                    }
                }
                else
                {
                    // Errors creating user
                    foreach (var error in result.Errors)
                    {
                        Error = error.Description;
                        break;
                    }
                    return Page();
                }
            }
            else
            {
                if (user.Status != UserStatus.Active)
                {
                    Error = $"La cuenta '{user.Email}' no está activa. Comunicate con soporte";
                    return Page();
                }
                else
                {
                    // User already have and account
                    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                    if (result.Succeeded)
                    {
                        if (user.PhoneNumber is null || user.NormalizedUserName == user.NormalizedEmail) return RedirectToPage("/QuickStartProfile", new { ReturnUrl });
                        else if (user.PhoneNumber is not null && !user.PhoneNumberConfirmed) return RedirectToPage("/ConfirmPhone", new { ReturnUrl });
                        else if (user.PhoneTwoFactorEnabled) return RedirectToPage("/TwoFactor", new { ReturnUrl, Remember = false });
                        else return new RedirectResult(ReturnUrl);

                    }
                    else if (result.IsNotAllowed)
                    {
                        // If account is not verified
                        Error = "Debes verificar tu cuenta, por favor revisa tu correo electronico";
                        AllowBack = true;
                        return Page();
                    }
                    else
                    {
                        Error = $"Se produjo un error al obtener tus datos de {info.ProviderDisplayName}.";
                        AllowBack = true;
                        return Page();
                    }

                }
            }
        }
    }

    public IActionResult OnPostToSignIn(string url)
    {
        return RedirectToPage("/Signin", new { ReturnUrl = url });
    }
    private IUserLoginStore<ApplicationUser> GetLogins()
    {
        return (IUserLoginStore<ApplicationUser>)_userStore;
    }
}