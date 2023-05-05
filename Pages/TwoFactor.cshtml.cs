using System.ComponentModel.DataAnnotations;
using IdentityProvider.Entity;
using IdentityProvider.Enumerables;
using IdentityProvider.Helper;
using IdentityProvider.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityProvider.Pages
{
    [ValidateAntiForgeryToken]
    public class TwoFactor : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsSender _smsSender;

        [BindProperty]
        [Display(Name = "Código de ingreso")]
        [Required(ErrorMessage = "Código de ingreso requerido.")]
        [MaxLength(8, ErrorMessage = "El código de ingreso no puede tener mas de 8 digitos")]
        [MinLength(6, ErrorMessage = "El código de ingreso no puede tener menos de 5 digitos")]
        public string Code { get; set; } = string.Empty;
        public string CodeSent { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public bool Remember { get; set; } = false;


        public TwoFactor(UserManager<ApplicationUser> userManager, ISmsSender smsSender, SignInManager<ApplicationUser> singIn)
        {
            _userManager = userManager;
            _smsSender = smsSender;
            _signInManager = singIn;
        }

        public async Task<IActionResult> OnGet(string returnUrl, bool? remember = null, string? token = null)
        {
            if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
            else ReturnUrl = returnUrl;
            if (remember != null) Remember = (bool)remember;
            else Remember = false;

            CodeSent = string.Empty;
            // We wnsure the user has already pass username and password authentication 
            ApplicationUser? user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Redirect("/Signin");
            }
            else
            {
                if (token is not null)
                {
                    switch (token)
                    {
                        case "Email":
                            CodeSent = $"Hemos enviado el código de ingreso a su correo electrónico {HideString.HideEmail(user.Email!)}.";
                            break;
                        case "Phone":
                            if (user.PhoneNumber is not null)
                            {
                                CodeSent = $"Hemos enviado el código de ingreso a su número de teléfono {HideString.HidePhone(user.PhoneNumber)}.";
                            }
                            else
                            {
                                CodeSent = $"Hemos enviado el código de ingreso, por favor revise su email o su número de teléfono.";
                            }
                            break;
                        default:
                            CodeSent = $"Hemos enviado el código de ingreso, por favor revise su email o su número de teléfono.";
                            break;
                    }
                }
                else
                {
                    CodeSent = $"Hemos enviado el código de ingreso, por favor revise su email o su número de teléfono";
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostToContinue(string returnUrl, bool? remember = null)
        {
            if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
            else ReturnUrl = returnUrl;
            if (remember != null) Remember = (bool)remember;
            else Remember = false;

            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {

                ApplicationUser? user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    return Redirect("/Signin");
                }
                else
                {
                    // if user is not active dont let him login
                    if (user.Status != UserStatus.Active)
                    {
                        Error = $"La cuenta '{HideString.HideEmail(user.Email!)}' no está activa. Comunicate con soporte.";
                        return Page();
                    }
                    string authenticatorCode = Code.Replace(" ", string.Empty).Replace("-", string.Empty).Trim();
                    SignInResult result;
                    if (user.PhoneNumber is null || !user.PhoneNumberConfirmed)
                    {
                        result = await _signInManager.TwoFactorSignInAsync("Email", authenticatorCode, Remember, Remember);
                    }
                    else
                    {
                        result = await _signInManager.TwoFactorSignInAsync("Phone", authenticatorCode, Remember, Remember);
                    }
                    if (result.Succeeded)
                    {
                        // If phone number is not set, redirect to page
                        if (user.PhoneNumber is null || user.NormalizedUserName == user.NormalizedEmail) return RedirectToPage("/QuickStartProfile", new { ReturnUrl });
                        else if (user.PhoneNumber is not null && !user.PhoneNumberConfirmed) return RedirectToPage("/ConfirmPhone", new { ReturnUrl });
                        else return new RedirectResult(ReturnUrl);
                    }
                    else if (result.IsNotAllowed)
                    {
                        // If account is not verified
                        Error = "Debes verificar tu cuenta, por favor revisa tu correo electrónico";
                        return Page();
                    }
                    else
                    {
                        Error = "Se produjo un error al iniciar sesión.";
                        return Page();
                    }
                }
            }
        }

        public async Task<IActionResult> OnPostToResend(string returnUrl, bool? remember, string? token = null)
        {
            if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
            else ReturnUrl = returnUrl;
            if (remember != null) Remember = (bool)remember;
            else Remember = false;
            ApplicationUser? user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Redirect("/Signin");
            }
            else
            {
                string Code;
                string Token;
                if (user.PhoneNumber is null || !user.PhoneNumberConfirmed)
                {
                    Token = "Email";
                    Code = await _userManager.GenerateTwoFactorTokenAsync(user, Token);
                    await _smsSender.PhoneConfirmation(user.Email!, Code);
                    CodeSent = $"Hemos enviado el código de ingreso a su correo electrónico {HideString.HideEmail(user.Email!)}";
                }
                else
                {
                    Token = "Phone";
                    Code = await _userManager.GenerateTwoFactorTokenAsync(user, Token);
                    await _smsSender.PhoneConfirmation(user.Email!, Code);
                    CodeSent = $"Hemos enviado el código de ingreso a su número de teléfono {HideString.HidePhone(user.PhoneNumber)}";
                    ModelState.Clear();
                }
                return Page();
            }
        }
    }
}
