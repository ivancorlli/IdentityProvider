using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using IdentityProvider.Constant;
using IdentityProvider.Entity;
using IdentityProvider.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = "Identity.Application", Roles = $"{DefaultRoles.ApplicationUser},{DefaultRoles.IdentityAdmin}")]
public class ConfirmPhone : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISmsSender _smsSender;

    [BindProperty]
    [Display(Name = "Código de verificación")]
    [Required(ErrorMessage = "Código de verificación requerido")]
    [MaxLength(8, ErrorMessage = "El código de verificación no puede tener mas de 8 digitos")]
    [MinLength(6, ErrorMessage = "El código de verificación no puede tener menos de 5 digitos")]
    public string Code { get; set; } = string.Empty;
    [BindProperty]
    [Display(Name = "Número de teléfono")]
    [Phone(ErrorMessage = "Número de teléfono invalido")]
    [Required(ErrorMessage = "Número de teléfono requerido")]
    [StringLength(20, ErrorMessage = "El número de teléfono debe tener entre 5 y 20 digitos", MinimumLength = 5)]
    public string PhoneNumber { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public bool AllowChange { get; set; } = false;
    public string AllowTitle { get; set; } = string.Empty;
    private const string VerificarTitle = "Verificar Teléfono";
    private const string UpdateTitle = "Actualizar Teléfono";
    public long ResendTime { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public ConfirmPhone(UserManager<ApplicationUser> userManager, ISmsSender smsSender)
    {
        _userManager = userManager;
        _smsSender = smsSender;
    }

    public async Task<IActionResult> OnGet(string returnUrl, long? ts)
    {
        if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
        else ReturnUrl = returnUrl;
        AllowTitle = VerificarTitle;
        if (ts is not null)
        {
            ResendTime = (long)ts;
        }
        else
        {
            ResendTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        AuthenticateResult auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (auth.Succeeded)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                if (user.PhoneNumber is null || user.NormalizedUserName == user.NormalizedEmail) return RedirectToPage("/QuickStartProfile", new { ReturnUrl });
                else if (user.PhoneNumber is not null && user.PhoneNumberConfirmed) return new RedirectResult(returnUrl);
                else
                {
                    return Page();
                }
            }
            else
            {
                // Delete all their cookies
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return RedirectToPage("/Signin", new { ReturnUrl });
            }
        }
        else
        {
            // Delete all their cookies
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToPage("/Signin", new { ReturnUrl });
        }
    }

    public async Task<IActionResult> OnPostToVerify(string returnUrl, long? ts)
    {
        if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
        else ReturnUrl = returnUrl;
        AllowTitle = VerificarTitle;
        if (ts is not null)
        {
            ResendTime = (long)ts;
        }
        else
        {
            ResendTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        // Skip validations for PhoneNumber
        if (!IsModelValidExcept("PhoneNumber"))
        {
            return Page();
        }
        else
        {
            AuthenticateResult auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (auth.Succeeded)
            {
                string userId = auth.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                ApplicationUser? user = await _userManager.FindByIdAsync(userId);
                if (user is not null)
                {
                    bool verified = await _userManager.VerifyChangePhoneNumberTokenAsync(user, Code, user.PhoneNumber!);
                    if (!verified)
                    {
                        Error = "El código ingresado es invalido.";
                        return Page();
                    }
                    else
                    {
                        user.PhoneNumberConfirmed = true;
                        // by default use two factor 
                        if (!user.IsAuthenticatedExternaly)
                        {
                            if (!user.TwoFactorEnabled)
                            {
                                user.UseTwoFactor();
                            }
                        }
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Errors.Count() > 0)
                        {
                            Error = result.Errors.First().Description;
                            return Page();
                        }
                        else
                        {
                            return Redirect(returnUrl);
                        }
                    }
                }
                else
                {
                    // Delete all their cookies
                    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                    return RedirectToPage("/Signin", new { ReturnUrl });
                }
            }
            else
            {
                // Delete all their cookies
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return RedirectToPage("/Signin", new { ReturnUrl });
            }
        }
    }

    public IActionResult OnPostToAllowChange(string returnUrl, long? ts)
    {
        AllowChange = true;
        AllowTitle = UpdateTitle;
        ReturnUrl = returnUrl;
        ModelState.Clear();
        if (ts is not null)
        {
            ResendTime = (long)ts;
        }
        else
        {
            ResendTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        ModelState.Clear();
        return Page();
    }

    public async Task<IActionResult> OnPostToResend(string returnUrl, long? ts)
    {
        AllowTitle = VerificarTitle;
        ReturnUrl = returnUrl;
        AllowChange = false;
        if (ts is not null)
        {
            ResendTime = (long)ts;
        }
        else
        {
            ResendTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        if(ResendTime > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            ModelState.Clear();
            return Page();
        }
        AuthenticateResult auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (auth.Succeeded)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                if (user.PhoneNumber is null || user.NormalizedUserName == user.NormalizedEmail) return RedirectToPage("/QuickStartProfile", new { ReturnUrl });
                else if (user.PhoneNumber is not null && user.PhoneNumberConfirmed) return new RedirectResult(returnUrl);
                else
                {
                    string Code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);
                    await _smsSender.PhoneConfirmation(user.Email!, Code);
                    ModelState.Clear();
                    ExecTimer();
                    return Page();
                }
            }
            else
            {
                // Delete all their cookies
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return RedirectToPage("/Signin", new { ReturnUrl });
            }
        }
        else
        {
            // Delete all their cookies
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToPage("/Signin", new { ReturnUrl });
        }
    }

    public IActionResult OnPostToCancel(string returnUrl, long? ts)
    {
        AllowTitle = VerificarTitle;
        AllowChange = false;
        ReturnUrl = returnUrl;
        ModelState.Clear();
        if (ts is not null)
        {
            ResendTime = (long)ts;
        }
        else
        {
            ResendTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        ModelState.Clear();
        return Page();
    }

    public async Task<IActionResult> OnPostToChangePhoneAsync(string returnUrl, long? ts)
    {
        AllowTitle = UpdateTitle;
        AllowChange = true;
        ReturnUrl = returnUrl;
        if (ts is not null)
        {
            ResendTime = (long)ts;
        }
        else
        {
            ResendTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        // Skip validations for Code
        if (!IsModelValidExcept("Code"))
        {
            return Page();
        }
        else
        {
            AuthenticateResult auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (auth.Succeeded)
            {
                string userId = auth.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                ApplicationUser? user = await _userManager.FindByIdAsync(userId);
                if (user is not null)
                {
                    if (PhoneNumber == user.PhoneNumber)
                    {
                        Error = "No puedes utilizar el mismo número de teléfono.";
                        return Page();
                    }

                    var result = await _userManager.SetPhoneNumberAsync(user, PhoneNumber.Trim());
                    if (result.Errors.Count() > 0)
                    {
                        Error = result.Errors.First().Description;
                        return Page();
                    }
                    string Code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, PhoneNumber.Trim());
                    await _smsSender.PhoneConfirmation(user.Email!, Code);
                    AllowTitle = VerificarTitle;
                    AllowChange = false;
                    ExecTimer();
                    ModelState.Clear();
                    return Page();
                }
                else
                {
                    // Delete all their cookies
                    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                    return RedirectToPage("/Signin", new { ReturnUrl });
                }
            }
            else
            {
                // Delete all their cookies
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return RedirectToPage("/Signin", new { ReturnUrl });
            }
        }

    }


    private bool IsModelValidExcept(string key)
    {
        if (ModelState.IsValid)
        {
            return true;
        }
        else
        {
            var response = true;
            foreach (var modelError in ModelState)
            {
                if (modelError.Value.Errors.Count > 0)
                {
                    if (modelError.Key == key)
                    {
                        response = true;
                    }
                    else
                    {
                        response = false;
                        break;
                    }
                }
            }
            return response;
        }
    }

    public void ExecTimer()
    {
        ResendTime = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
    }
}