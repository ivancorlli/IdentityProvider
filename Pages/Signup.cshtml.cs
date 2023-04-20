using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using IdentityProvider.Interface;
using System.Text;
using Microsoft.Extensions.Options;
using IdentityProvider.Options;

namespace IdentityProvider.Pages
{
    public class SignupModel : PageModel
    {
        [BindProperty]
        public ModelRegister Register { get; set; } = new ModelRegister();
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly IOptions<ReturnUrlOptions> _defaultReturnUrl;
        private readonly IEmailSender _emailSender;

        public string ReturnUrl { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public SignupModel(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
			IOptions<ReturnUrlOptions> returnUrl 
            )
        {
            _userManager = userManager;
            _emailSender = emailSender;
			_defaultReturnUrl = returnUrl;
        }

        public IActionResult? OnGet(string returnUrl)
        {
            if( returnUrl == null) {
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
            
            if (ModelState.IsValid)
            {
				var user = new ApplicationUser { Email = Register.Email.Trim(), UserName = Register.Email.ToLower().Trim() };
                var result = await _userManager.CreateAsync(user, Register.Password);
                if (result.Succeeded)
                {
                    var UserId = await _userManager.GetUserIdAsync(user);
                    var Code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Code));
                    var Exp = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
                    var callback = Url.Page(
                        pageName: "/ConfirmEmail",
                        pageHandler: null,
                        values: new
                        {
                            UserId,
                            Code,
							ReturnUrl = returnUrl,
                            Exp = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Exp.ToString())),
                        },
                        protocol: Request.Scheme
                        );
                    await _emailSender.SendWelcome(user.Email.ToString());
                    await _emailSender.SendConfirmationEmail(user.Email.ToString(), callback!); ;
                    ModelState.Clear();
                    return RedirectToPage("/SignupConfirmation", new { Email = user.Email.ToString(), ReturnUrl });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Error = error.Description;
                        break;
                    }
                }
            }
            else
            {
                foreach (var modelError in ModelState)
                {
                    if (modelError.Key != "Error" && modelError.Value.Errors.Count > 0)
                    {
                        Error = modelError.Value.Errors.First().ErrorMessage.ToString();
                        break;
                    }
                }
                ModelState.Clear();
                return Page();
            }
            return Page();
        }

        public IActionResult OnPostToSignIn(string url)
        {
            return RedirectToPage("/Signin", new {ReturnUrl = url});
        }
    }
}
