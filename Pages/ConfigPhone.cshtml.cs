using System.Security.Cryptography.X509Certificates;
using System.Text;
using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityProvider.Pages
{
    public class ConfigPhone : PageModel
    {
        [BindProperty]
        public ConfigPhoneModel PhoneModel {get;set;} = new();

        private readonly UserManager<ApplicationUser> _userManager;
        public string ReturnUrl {get;set;} = string.Empty;
        public string Error {get;set;} = string.Empty;
    
        public ConfigPhone(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet(string ue,string returnUrl)
        {
            if(
                string.IsNullOrEmpty(returnUrl)||
                string.IsNullOrEmpty(ue)
            )
            {
                return Redirect("/Signin");
            }else {
                ReturnUrl = returnUrl;
                return Page();
            }
        }


        public async void OnPost(string ue,string returnUrl)
        {
            var userId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(ue));
            var user = await _userManager.FindByIdAsync(userId);
            var x = await _userManager.GenerateTwoFactorTokenAsync(user!,"Phone");
            
        }
    }
}