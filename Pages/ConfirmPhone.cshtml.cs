using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using IdentityProvider.Constant;
using IdentityProvider.Entity;
using IdentityProvider.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages
{
    [Authorize(AuthenticationSchemes = "Identity.Application", Roles = $"{DefaultRoles.ApplicationUser},{DefaultRoles.IdentityAdmin}")]
    public class ConfirmPhone : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsSender _smsSender;

        [BindProperty]
        [Display(Name = "Codigo de verificacion")]
        [Required(ErrorMessage = "Codigo de verificacion requerido")]
        [MaxLength(8, ErrorMessage = "El codigo de verificacion no puede tener mas de 8 digitos")]
        [MinLength(6, ErrorMessage = "El codigo de verificacion no puede tener menos de 5 digitos")]
        public string Code { get; set; } = string.Empty;
        [BindProperty]
        [Display(Name = "Número de teléfono")]
        [Phone(ErrorMessage = "Número de teléfono invalido")]
        [Required(ErrorMessage = "Número de teléfono requerido")]
        [MaxLength(20, ErrorMessage = "El número de teléfono no puede tener mas de 20 digitos")]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public bool AllowChange { get; set; } = false;
        public string AllowTitle { get; set; } = string.Empty;

        public ConfirmPhone(UserManager<ApplicationUser> userManager, ISmsSender smsSender)
        {
            _userManager = userManager;
            _smsSender = smsSender;
        }

        public async Task<IActionResult> OnGet(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
            else ReturnUrl = returnUrl;
            AllowTitle = "Verificar numero de telefono";
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

        public async Task<IActionResult> OnPostToVerify(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
            else ReturnUrl = returnUrl;
            AllowTitle = "Verificar numero de telefono";
            foreach (var state in ModelState.Where(x => x.Key == "PhoneNumber").Select(x => x.Value))
            {
                state!.Errors.Clear();
                state.ValidationState = ModelValidationState.Skipped;
            }
            if (ModelState.IsValid)
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
                            Error = "El codigo ingresado es invalido.";
                            return Page();
                        }
                        else
                        {
                            user.PhoneNumberConfirmed = true;
                            // by default use two factor 
                            if (!user.IsAuthenticatedExternaly) user.UseTwoFactor();
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
            else
            {
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

        public IActionResult OnPostToAllowChange(string returnUrl)
        {
            AllowChange = true;
            AllowTitle = "Actualizar numero de telefono";
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostToResend(string returnUrl)
        {
            AllowTitle = "Verificar numero de telefono";
            ReturnUrl = returnUrl;
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

        public IActionResult OnPostToCancel(string returnUrl)
        {
            AllowTitle = "Verificar numero de telefono";
            AllowChange = false;
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostToChangePhoneAsync(string returnUrl)
        {
            AllowTitle = "Actualizar numero de telefono";
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
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
                            Error = "No puedes utilizar el mismo numero de telefono.";
                            return Page();
                        }

                        var result = await _userManager.SetPhoneNumberAsync(user, PhoneNumber.Trim());
                        if (result.Errors.Count() > 0)
                        {
                            Error = result.Errors.First().Description;
                            return Page();
                        }
                        string Code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);
                        await _smsSender.PhoneConfirmation(user.Email!, Code);
                        AllowTitle = "Verificar numero de telefono";
                        AllowChange = false;
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
            else
            {
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

    }
}