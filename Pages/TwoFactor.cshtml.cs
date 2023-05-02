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

namespace IdentityProvider.Pages
{
    [Authorize(AuthenticationSchemes = "Identity.Application", Roles = $"{DefaultRoles.ApplicationUser},{DefaultRoles.IdentityAdmin}")]
    public class TwoFactor : PageModel
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


        public TwoFactor(UserManager<ApplicationUser> userManager, ISmsSender smsSender)
        {
            _userManager = userManager;
            _smsSender = smsSender;
        }

        public async Task<IActionResult> OnGet(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
            else ReturnUrl = returnUrl;
            AuthenticateResult auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (auth.Succeeded)
            {
                string userId = auth.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                ApplicationUser? user = await _userManager.FindByIdAsync(userId);
                if (user is not null)
                {
                    var Code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);
                    await _smsSender.PhoneConfirmation(user.Email!, Code);
                    return Page();
                }
                else return RedirectToPage("/Signin", new { ReturnUrl });
            }
            else return RedirectToPage("/Signin", new { ReturnUrl });
        }

        public async Task<IActionResult> OnPost(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
            else ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {

                AuthenticateResult auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
                if (auth.Succeeded)
                {
                    string userId = auth.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                    ApplicationUser? user = await _userManager.FindByIdAsync(userId);
                    if (user is not null)
                    {
                        var verified = await _userManager.VerifyChangePhoneNumberTokenAsync(user, Code, user.PhoneNumber!);
                        if (!verified)
                        {
                            Error = "El codigo ingresado es invalido.";
                            return Page();
                        }
                        else
                        {
                            user.PhoneNumberConfirmed = true;
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
                    else return RedirectToPage("/Signin", new { ReturnUrl });
                }
                else return RedirectToPage("/Signin", new { ReturnUrl });
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
