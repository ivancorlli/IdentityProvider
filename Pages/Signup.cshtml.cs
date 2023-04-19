using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using IdentityProvider.Interface;
using System.Text;
using System.Text.Encodings.Web;
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

        public void OnGet(string returnUrl)
        {
            var url = HttpContext.Request.QueryString.Value!;
            if (url.Contains("?ReturnUrl="))
            {
                var split = url.Split("?ReturnUrl=");
                ReturnUrl = split[1];
            }
            else if (url.Contains("?returnUrl="))
            {
                var split = url.Split("?returnUrl=");
                ReturnUrl = split[1];
            }
            else if (url.Contains("?returnurl="))
            {
                var split = url.Split("?returnurl=");
                ReturnUrl = split[1];
            }
            else
            {
                ReturnUrl = returnUrl;
            }

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var url = HttpContext.Request.QueryString.Value!;
                if (url.Contains("?ReturnUrl="))
                {
                    var split = url.Split("?ReturnUrl=");
                    ReturnUrl = split[1];
                }
                else if (url.Contains("?returnUrl="))
                {
                    var split = url.Split("?returnUrl=");
                    ReturnUrl = split[1];
                }
                else if (url.Contains("?returnurl="))
                {
                    var split = url.Split("?returnurl=");
                    ReturnUrl = split[1];
                }
                else
                {
                    ReturnUrl = returnUrl;
                }

                var user = new ApplicationUser { Email = Register.Email.Trim(), UserName = Register.Email.ToLower().Trim() };
                var result = await _userManager.CreateAsync(user, Register.Password);
                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var exp = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
                    var callback = Url.Page(
                        pageName: "/ConfirmEmail",
                        pageHandler: null,
                        values: new
                        {
                            userId,
                            code,
							returnUrl = _defaultReturnUrl.Value.Default,
                            exp = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(exp.ToString())),
                        },
                        protocol: Request.Scheme
                        );
                    // Send Email
                    // await _emailSender.SendWelcome(user.Email.ToString());
                    await _emailSender.SendConfirmationEmail(user.Email.ToString(), callback!); ;
                    ModelState.Clear();
                    return RedirectToPage("/SignupConfirmation", new { email = user.Email, ReturnUrl });
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
            return Redirect($"/signin?ReturnUrl={url}");
        }
    }
}
