using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using IdentityProvider.Interface;
using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;

namespace IdentityProvider.Pages
{
	public class RecoveryModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IEmailSender _emailSender;
		public ModelRecovery Recovery { get; set; } = new ModelRecovery();

		public RecoveryModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
		{
			_userManager = userManager;
			_emailSender = emailSender;
		}
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(Recovery.Email);
				if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return RedirectToPage("ForgotPasswordConfirmation");
				}

				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var callbackUrl = Url.Page(
					"/ResetPassword",
					pageHandler: null,
					values: new { area = "Identity", code },
					protocol: Request.Scheme)!;

				//await _emailSender.SendEmailAsync(
				//    Input.Email,
				//    "Reset Password",
				//    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

				return RedirectToPage("/ForgotPasswordConfirmation");
			}

			return Page();
		}

		public IActionResult OnPostToSignUp()
		{
			return Redirect("/signin");
		}

	}
}
