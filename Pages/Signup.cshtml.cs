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
using IdentityProvider.Constant;

namespace IdentityProvider.Pages
{
    public class SignupModel : PageModel
    {
        [BindProperty]
        public ModelRegister Register { get; set; } = new ModelRegister();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly IOptions<ReturnUrlOptions> _defaultReturnUrl;
        private readonly IEmailSender _emailSender;

        public string ReturnUrl { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public SignupModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IEmailSender emailSender,
			IOptions<ReturnUrlOptions> returnUrl 
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
                // Create a new user and give him some credentials, it's very important to initialize it with false for 'IsAuthenticatedExternaly', It's set TwoFactor authentication enabled for default.
				ApplicationUser user = ApplicationUser.CreateLocalUser(Register.Email);
                IdentityResult result = await _userManager.CreateAsync(user, Register.Password);
                if (result.Succeeded)
                {
                    // Aggreagate a rol, bydeafult an application role
                    bool role = await _roleManager.RoleExistsAsync(DefaultRoles.ApplicationUser);
                    if(role)
                    {
                        await _userManager.AddToRoleAsync(user,DefaultRoles.ApplicationUser);
                    }else {
                        await _userManager.AddToRoleAsync(user,DefaultRoles.DefaultUser);
                    }

                    string UserId = await _userManager.GetUserIdAsync(user);
                    string Code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Code));
                    long Exp = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();
                    string callback = Url.Page(
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
                        )!;
                    // Send welcom email
                    await _emailSender.SendWelcome(user.Email!.ToString());
                    // Send confirmation email
                    await _emailSender.SendConfirmationEmail(user.Email.ToString(), callback!);
                    ModelState.Clear();
                    return RedirectToPage("/SignupConfirmation", new { Email = user.Email.ToString(), ReturnUrl });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if(error.Code == "DuplicateUserName")
                        {
                            Error = $"El correo electrónico '{Register.Email}' ya ha sido registrado.";
                        }else if(error.Code == "DuplicateEmail"){
                            Error = $"El correo electrónico '{Register.Email}' ya ha sido registrado.";
                        }else {
                            Error = error.Description;
                        }
                        break;
                    }
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
            return Page();
        }

        public IActionResult OnPostToSignIn(string url)
        {
            return RedirectToPage("/Signin", new {ReturnUrl = url});
        }
    }
}
