using IdentityProvider.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages
{
	public class ConfirmationModel : PageModel
	{
		public string? Email { get; set; }
		private readonly UserManager<ApplicationUser> _userManager;
		public string ReturnUrl {get;set;} = string.Empty;
		public ConfirmationModel(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IActionResult> OnGet(string email,string returnUrl)
		{
			if (email == null)
			{
				return RedirectToPage("/signin");
			}
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return NotFound($"No pudimos cargar al usuario '{email}'.");
			}

			Email = email;
			ReturnUrl = returnUrl;
			return Page();
		}
		public IActionResult OnPostToSignIn(string url)
		{
			return Redirect($"/signin?ReturnUrl={url}");
		}
	}
}
