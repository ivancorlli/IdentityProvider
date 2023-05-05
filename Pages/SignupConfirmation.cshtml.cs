using System.Text;
using IdentityProvider.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityProvider.Pages
{
	[ValidateAntiForgeryToken]
	public class ConfirmationModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		public string ReturnUrl {get;set;} = string.Empty;
		public ConfirmationModel(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IActionResult> OnGet(string ue,string returnUrl)
		{
			if(string.IsNullOrEmpty(returnUrl)) {
				return Redirect("/Signin");
			}else
			{
				ReturnUrl = returnUrl;
			}

			if (ue == null)
			{
				return RedirectToPage("/Signin",new { ReturnUrl});
			}
			var userId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(ue));
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return RedirectToPage("/Signin",new { ReturnUrl});
			}

			return Page();
		}
		public IActionResult OnPostToSignIn(string url)
		{
			return RedirectToPage("/Signin",new { ReturnUrl = url });
		}
	}
}
