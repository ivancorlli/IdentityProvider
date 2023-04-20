using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IdentityProvider.Pages
{
	public class ResetPasswordModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		[BindProperty]
		public ModelResetPassword Reset { get; set; } = new ModelResetPassword();
		public string Error {  get; set; } =string.Empty;
		public string ReturnUrl { get; set; } = string.Empty;
		public ResetPasswordModel(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IActionResult> OnGet(string ue,string code,string exp,string returnUrl)
		{
			if (string.IsNullOrEmpty(ue)|| 
				string.IsNullOrEmpty(code) ||
				string.IsNullOrEmpty(exp) ||
				string.IsNullOrEmpty(returnUrl)
				)
			{ 
				return Redirect("/Signin");
			}
			else
			{
				ReturnUrl = returnUrl;
			}

			var userId  = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(ue));
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return RedirectToPage("/Signin", new { ReturnUrl });
			}

			if(!user.EmailConfirmed)
			{
				return RedirectToPage("/Signin", new { ReturnUrl });
			}

			exp = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(exp));
			var ExpirationTime = long.Parse(exp);
			if (ExpirationTime > 0)
			{
				var Now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				if (Now > ExpirationTime)
				{
					Error = "No puedes acceder a este recurso";
				}
			}
			
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(string ue, string code, string exp, string returnUrl)
		{

			ReturnUrl = returnUrl;
			var userId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(ue));
			code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToPage("/ResetPasswordConfirmation");
			}

			var result = await _userManager.ResetPasswordAsync(user, code, Reset.Password);
			if (result.Succeeded)
			{
				return RedirectToPage("/ResetPasswordConfirmation");
			}
			else
			{
				Error = "Se produjo un erro al actualizar contraseña";
				return Page();
			}
		}
	}
}
