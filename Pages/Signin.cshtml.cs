using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly IUserLoginStore<ApplicationUser> _userLogin;
		public string? ReturnUrl { get; set; } = string.Empty;
		public string Error { get; set; } = string.Empty;
		public List<AuthenticationScheme> ExternalLogins { get; set; } = new();
		[BindProperty]
		public LoginModel Login { get; set; } = new LoginModel();

		public SignInModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signIn,
			IOptions<ReturnUrlOptions> retunrn,
			IUserStore<ApplicationUser> userStore
			)
		{
			_userManager = userManager;
			_signIn = signIn;
			_defaultReturnUrl = retunrn;
			_userStore = userStore;
			_userLogin = GetLogins();
		}

		public async Task<IActionResult?> OnGetAsync(string? returnUrl = null)
		{

			if (string.IsNullOrEmpty(returnUrl))
			{
				ReturnUrl = _defaultReturnUrl.Value.Default.ToString();
			}
			else
			{
				ReturnUrl = returnUrl;
			}

			var auth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
			if (auth.Succeeded)
			{
				return new RedirectResult(ReturnUrl);

			}

			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
			var list = await _signIn.GetExternalAuthenticationSchemesAsync();
			ExternalLogins = list.ToList();
			return null;
		}

		public async Task<IActionResult> OnPost(string? returnUrl = null)
		{
			if (string.IsNullOrEmpty(returnUrl))
			{
				ReturnUrl = _defaultReturnUrl.Value.Default.ToString();
			}
			else
			{
				ReturnUrl = returnUrl;
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
					if (string.IsNullOrEmpty(user.PasswordHash))
					{
						// Buscamos los logins externos del usuario
						var providers = await _userLogin.GetLoginsAsync(user, CancellationToken.None);
						if (providers != null)
						{
							if (providers.Count > 0)
							{
								var message = "";
								if (providers.Count == 1)
								{
									message = providers[0].ProviderDisplayName!.ToString();
								}
								else
								{

									foreach (var provider in providers)
									{
										message += $"{provider.ProviderDisplayName}, ";
									}
								}
								Error = $"Puedes iniciar sesion con {message}";
								return Page();
							}
							else
							{
								Error = "Usuario inexistente";
								return Page();
							}
						}
						else
						{
							Error = "Usuario inexistente";
							return Page();
						}
					}

					var result = await _signIn.PasswordSignInAsync(user, Login.Password, Login.Remember, lockoutOnFailure: false);
					if (result.Succeeded)
					{
						return Redirect(ReturnUrl);
					}
					else if (result.IsNotAllowed)
					{

						Error = "Debes verificar tu cuenta, por favor revisa tu correo electronico";
						return Page();

					}
					else
					{
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
			return RedirectToPage("/Signup", new { ReturnUrl = url });
		}
		private IUserLoginStore<ApplicationUser> GetLogins()
		{
			return (IUserLoginStore<ApplicationUser>)_userStore;
		}
	}
}