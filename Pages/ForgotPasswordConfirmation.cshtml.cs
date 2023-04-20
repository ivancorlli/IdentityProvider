using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages;

public class ForgotPasswordConfirmationModel : PageModel
{
	public string ReturnUrl { get; set; } = string.Empty;
	public IActionResult? OnGet(string returnUrl)
	{
		if(string.IsNullOrEmpty(returnUrl)) {

			return Redirect("/Signin");

		}else
		{
			ReturnUrl = returnUrl;
			return null;
		}
	}


	public IActionResult OnPostToSignUp(string url)
	{
		return RedirectToPage("/Signin", new { ReturnUrl = url });
	}
}
