using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using IdentityProvider.Interface;
using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using IdentityProvider.Enumerables;
using IdentityProvider.Helper;

namespace IdentityProvider.Pages;

[ValidateAntiForgeryToken]
public class RecoveryModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserLoginStore<ApplicationUser> _userLogin;
    [BindProperty]
    public ModelRecovery Recovery { get; set; } = new ModelRecovery();
    public string ReturnUrl { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;

    public RecoveryModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IUserStore<ApplicationUser> userStore)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _userStore = userStore;
			_userLogin =GetLogins();
    }

    public IActionResult? OnGet(string returnUrl)
    {
        if (returnUrl == null)
        {
            return Redirect("/Signin");
        }
        else
        {
            ReturnUrl = returnUrl;
            return null;
        }

    }


    public async Task<IActionResult> OnPostAsync(string returnUrl)
    {

        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }
        else
        {
            var user = await _userManager.FindByEmailAsync(Recovery.Email);
            if (user is null)
            {
                Error = "Usuario inexistente";
                return Page();
            }
            else if (!user.EmailConfirmed)
            {
                Error = "Tu cuenta no está verificada. Revisa tu correo electronico.";
                return Page();
            }
            else if (user.PasswordHash == null)
            {
                // Buscamos los logins externos del usuario
                var providers = await _userLogin.GetLoginsAsync(user, CancellationToken.None);
                if (providers != null)
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
                        Error = $"No puedes acceder a este recurso. Puedes iniciar sesion con {message}.";
                        return Page();
                    }
                    else
                    {
                        Error = "Usuario inexistente.";
                        return Page();
                    }
                }
                else
                {
                    Error = "Usuario inexistente.";
                    return Page();
                }
            }else if(user.Status != UserStatus.Active)
            {
                Error = $"La cuenta {HideString.HideEmail(user.Email!)} no está activa. Comunicate con soporte.";
                return Page();
            }
            else
            {

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var Exp = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();
                var callbackUrl = Url.Page(
                    "/ResetPassword",
                    pageHandler: null,
                    values: new
                    {
                        UE = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id.ToString())),
                        Code = code,
                        Exp = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Exp.ToString())),
                        ReturnUrl
                    },
                    protocol: Request.Scheme)!;

                await _emailSender.SendResetPassword(user.Email!, callbackUrl);
                return RedirectToPage("/ForgotPasswordConfirmation", new { ReturnUrl });
            }
        }
    }

    public IActionResult OnPostToSignUp(string url)
    {
        return RedirectToPage("/Signin", new { ReturnUrl = url });
    }

    private IUserLoginStore<ApplicationUser> GetLogins()
    {
        return (IUserLoginStore<ApplicationUser>)_userStore;
    }
}
