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
		public ModelResetPassword Reset { get; set; } = new ModelResetPassword();

		public ResetPasswordModel(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public IActionResult OnGet(string? code = null)
		{
			if (code == null)
			{
				return BadRequest("A code must be supplied for password reset.");
			}
			else
			{
				Reset = new ModelResetPassword
				{
					Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
				};
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await _userManager.FindByEmailAsync(Reset.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToPage("/ResetPasswordConfirmation");
			}

			var result = await _userManager.ResetPasswordAsync(user, Reset.Code, Reset.Password);
			if (result.Succeeded)
			{
				return RedirectToPage("/ResetPasswordConfirmation");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
			return Page();
		}
	}
}
