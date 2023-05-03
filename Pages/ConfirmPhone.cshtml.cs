using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using IdentityProvider.Constant;
using IdentityProvider.Entity;
using IdentityProvider.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

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
        public string Error { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public bool AllowChange {get;set;} = false;
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

        public async Task<IActionResult> OnPost(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
            else ReturnUrl = returnUrl;
            AllowTitle = "Verificar numero de telefono";
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
                            user.UseTwoFactor();
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

        public IActionResult OnPostToAllowChange()
        {
            AllowChange = true;
            AllowTitle = "Actualizar numero de telefono";
            return Page();
        }

        public IActionResult OnPostToCancel()
        {
            AllowTitle = "Verificar numero de telefono";
            AllowChange = false;
            return Page();
        }
    }
}