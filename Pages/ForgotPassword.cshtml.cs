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

		[BindProperty]
		public ModelRecovery Recovery { get; set; } = new ModelRecovery();
		public string ReturnUrl { get; set; } = string.Empty;

		public RecoveryModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
		{
			_userManager = userManager;
			_emailSender = emailSender;
		}

		public IActionResult? OnGet(string returnUrl)
		{
			if(returnUrl == null)
			{
				return Redirect("/Signin");
			}else
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
				var user = await _userManager.FindByEmailAsync(Recovery.Email);
				if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
				{
					return RedirectToPage("/ForgotPasswordConfirmation", new {ReturnUrl});
				}

				if(user.PasswordHash == null) 
				{
					return RedirectToPage("/Signin",new {ReturnUrl});
				}
			
				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var Exp = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();
				var callbackUrl = Url.Page(
					"/ResetPassword",
					pageHandler: null,
					values: new { 
						UE = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id.ToString())), 
						Code=code,
						Exp = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Exp.ToString())),
						ReturnUrl
					},
					protocol: Request.Scheme)!;

				await _emailSender.SendResetPassword(user.Email!,callbackUrl);
				return RedirectToPage("/ForgotPasswordConfirmation", new { ReturnUrl });
			}

			return Page();
		}

		public IActionResult OnPostToSignUp(string url)
		{
			return RedirectToPage("/Signin", new {ReturnUrl = url});
		}

	}
}
