using System.Security.Claims;
using System.Text;
using IdentityProvider.Constant;
using IdentityProvider.Entity;
using IdentityProvider.Interface;
using IdentityProvider.Manager;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityProvider.Pages
{
    [Authorize(AuthenticationSchemes = "Identity.Application", Roles = $"{DefaultRoles.ApplicationUser},{DefaultRoles.IdentityAdmin}")]
    public class QuickStartProfile : PageModel
    {

        [BindProperty]
        public ProfileModel Profile { get; set; } = new();

        private readonly ApplicationManager _userManager;
        public string ReturnUrl { get; set; } = string.Empty;
        public string DefaultImage { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public QuickStartProfile(ApplicationManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) return Redirect("/Signin");
            else ReturnUrl = returnUrl;

            AuthenticateResult auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (auth.Succeeded)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                ApplicationUser? user = await _userManager.FindByIdAsync(userId);
                if (user is not null)
                {
                    if (user.PhoneNumber is not null && user.NormalizedUserName != user.NormalizedEmail && !user.PhoneNumberConfirmed)
                    {
                        return RedirectToPage("/ConfirmPhone", new { ReturnUrl });
                    }
                    else if (user.PhoneNumber is not null && user.NormalizedEmail != user.NormalizedUserName && user.PhoneNumberConfirmed)
                    {
                        return new RedirectResult(ReturnUrl);
                    }
                    else
                    {
                        UserProfile? profile = await _userManager.GetUserProfile(userId);
                        if (user.UserName is not null) if (user.UserName != user.Email) Profile.UserName = user.UserName;
                        if (user.PhoneNumber is not null) Profile.PhoneNumber = user.PhoneNumber;
                        if (profile is not null) if (profile.ProfilePicture is not null) DefaultImage = profile.ProfilePicture;
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

            if (ModelState.IsValid)
            {
                if (Profile.UserName.Contains("@"))
                {
                    Error = "El formato del nombre de usuario es invalido.";
                    return Page();
                }

                AuthenticateResult auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
                if (auth.Succeeded)
                {
                    string userId = auth.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                    ApplicationUser? user = await _userManager.FindByIdAsync(userId);
                    if (user is not null)
                    {
                        if (Profile.UserName == user.Email)
                        {
                            Error = "El formato del nombre de usuario es invalido.";
                            return Page();
                        }

                        var result = await _userManager.SetUserNameAsync(user, Profile.UserName);
                        if (result.Errors.Count() > 0)
                        {
                            Error = result.Errors.First().Description;
                            return Page();
                        }
                        result = await _userManager.SetPhoneNumberAsync(user, Profile.PhoneNumber);
                        if (result.Errors.Count() > 0)
                        {
                            Error = result.Errors.First().Description;
                            return Page();
                        }
                        return RedirectToPage("/ConfirmPhone", new { ReturnUrl });
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