using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages
{
    public class ConfigPhone : PageModel
    {
        [BindProperty]
        public ConfigPhoneModel PhoneModel { get; set; } = new();

        private readonly UserManager<ApplicationUser> _userManager;
        public string ReturnUrl { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public ConfigPhone(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize()]
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

            // var auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            // if(auth.Succeeded)
            // {
                 return Page();
            // }else {
            //     return RedirectToPage("/Signin", new {ReturnUrl});
            // }
            
        }


        [Authorize()]
        public async Task<IActionResult> OnPost(string returnUrl)
        {
            var auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if(auth.Succeeded)
            {
                return Page();
            }else {
                return RedirectToPage("/Signin", new {ReturnUrl});
            }

        }
    }
}