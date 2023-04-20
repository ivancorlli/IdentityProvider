using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityProvider.Interface;
using Microsoft.Extensions.Options;
using IdentityProvider.Options;
using Microsoft.AspNetCore.Authentication;

namespace IdentityProvider.Pages
{
    public class SignInModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<ReturnUrlOptions> _defaultReturnUrl;
        private readonly IEmailSender _emailSender;
        public string? ReturnUrl { get; set; } = string.Empty;
        public string? SocialReturn {get;set;} = string.Empty;
        public string Error { get; set; } = string.Empty;
        public List<AuthenticationScheme> ExternalLogins {get;set;} = new();
        [BindProperty]
        public LoginModel Login { get; set; } = new LoginModel();

        public SignInModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signIn,
            IEmailSender emailSender,
            IOptions<ReturnUrlOptions> retunrn
            )
        {
            _userManager = userManager;
            _signIn = signIn;
            _emailSender = emailSender;
            _defaultReturnUrl = retunrn;
        }

        public async Task<IActionResult?> OnGetAsync(string? returnUrl = null)
        {

            if (string.IsNullOrEmpty(returnUrl))
            {
                ReturnUrl = Url.Content(_defaultReturnUrl.Value.Default);
                SocialReturn = Url.Content(_defaultReturnUrl.Value.Default);
            }
            else
            {
                var url = HttpContext.Request.QueryString.Value!;
                SocialReturn = returnUrl;
                if (url.Contains("?ReturnUrl="))
                {
                    var split = url.Split("?ReturnUrl=");
                    ReturnUrl = Url.Content(split[1]);
                }
                else if (url.Contains("?returnUrl="))
                {
                    var split = url.Split("?returnUrl=");
                    ReturnUrl = Url.Content(split[1]);
                }
                else if (url.Contains("?returnurl="))
                {
                    var split = url.Split("?returnurl=");
                    ReturnUrl = Url.Content(split[1]);
                }
                else
                {
                    ReturnUrl = _defaultReturnUrl.Value.Default;
                }
            }

            if(HttpContext.User.Identity != null) 
                if(HttpContext.User.Identity.IsAuthenticated)
                {
                    return Redirect(ReturnUrl);
                }
            
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var list = await _signIn.GetExternalAuthenticationSchemesAsync();
            ExternalLogins = list.ToList();
            return null;
        }

        public async Task<IActionResult> OnPost(string? returnUrl=null)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                ReturnUrl = Url.Content(_defaultReturnUrl.Value.Default);

            }
            else
            {
                ReturnUrl = Url.Content(returnUrl);

            }
            if (ModelState.IsValid)
            {
                var list = await _signIn.GetExternalAuthenticationSchemesAsync();
                ExternalLogins = list.ToList();

                ApplicationUser? user;
                if (Login.Email.Contains(char.Parse("@")))
                {
                    // Buscamos al usuario por su email
                    var email = _userManager.NormalizeEmail(Login.Email);
                    user = await _userManager.FindByEmailAsync(email);
                }
                else
                {
                    // Buscamos al usuario por su nombre de usuario
                    user = await _userManager.FindByNameAsync(Login.Email);
                }

                if (user != null)
                {
                    // Si la contrasenia es null, entonces existe el usuario pero resgistrado con un provedor
                    if(string.IsNullOrEmpty(user.PasswordHash))
                    {
                        Error = "Usuario inexistente";
                        return Page();
                    }
                    var result = await _signIn.PasswordSignInAsync(user, Login.Password, Login.Remember, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        if (result.IsNotAllowed)
                        {

                            Error = "Debes verificar tu cuenta, por favor revisa tu correo electronico";
                            return Page();

                        };
                        Error = "Contraseña incorrecta";
                        return Page();
                    }
                }
                else
                {
                    Error = "Usuario inexistente";
                    return Page();
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


        public IActionResult OnPostToSignUp(string url)
        {
            return Redirect($"/signup?ReturnUrl={Url.Content(url)}");
        }
    }
}