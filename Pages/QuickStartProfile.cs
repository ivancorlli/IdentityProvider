using System.Security.Claims;
using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages
{
    [Authorize(AuthenticationSchemes = "Identity.Application", Roles = "ApplicationUser,ProviderAdmin")]
    public class QuickStartProfile : PageModel
    {
        [BindProperty]
        public ProfileModel Profile { get; set; } = new();

        private readonly UserManager<ApplicationUser> _userManager;
        public string ReturnUrl { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public QuickStartProfile(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            if (
                string.IsNullOrEmpty(returnUrl)
            )
            {
                return Redirect("/Signin");
            }
            else
            {
                ReturnUrl = returnUrl;
            }

            AuthenticateResult auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (auth.Succeeded)
            {
                string userId = auth.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                ApplicationUser? user = await _userManager.FindByIdAsync(userId);
                if(user is not null)
                {
                    return Page();
                }else{
                    return RedirectToPage("/Signin", new { ReturnUrl });
                }
            }
            else
            {
                return RedirectToPage("/Signin", new { ReturnUrl });
            }

        }

        // public async Task<IActionResult> OnPost(string returnUrl)
        // {
        //     if (
        //     string.IsNullOrEmpty(returnUrl)
        // )
        //     {
        //         return Redirect("/Signin");
        //     }
        //     else
        //     {
        //         ReturnUrl = returnUrl;
        //     }

        // }
    }
}