using IdentityProvider.Entity;
using IdentityProvider.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IdentityProvider.Pages;

public class ConfirmEmailModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IEmailSender _emailSender;
	public string StatusMessage { get; set; } = string.Empty;
	public string ErrorMessage { get; set; } = string.Empty;
	public string ReturnUrl { get; set; } = string.Empty;
	public string UserId { get; set; } = string.Empty;
	public bool Resend { get;set; } =false;
	public ConfirmEmailModel(UserManager<ApplicationUser> userManager,IEmailSender sender)
	{
		_userManager = userManager;
		_emailSender = sender;
	}

	public async Task<IActionResult> OnGetAsync(string userId, string code, string returnUrl, string exp)
	{
		if (
			string.IsNullOrEmpty(userId) ||
			string.IsNullOrEmpty(code) ||
			string.IsNullOrEmpty(returnUrl) ||
			string.IsNullOrEmpty(exp) 
			)
		{
			return RedirectToPage("/Signin");
		}else
		{
			ReturnUrl = returnUrl;
		}

		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
		{
			return RedirectToPage("/Signin", new {ReturnUrl});
		}

		UserId = user.Id;
		if (user.EmailConfirmed) {
			return RedirectToPage($"/Signin",new { ReturnUrl });
		}

		exp = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(exp));
		var ExpirationTime = long.Parse(exp);
		if(ExpirationTime > 0 )
		{
			var Now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			if(Now > ExpirationTime)
			{
				ErrorMessage = "Verificacion expirada";
				if (!user.EmailConfirmed) Resend = true;
				return Page();
			}
		}


		code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
		var result = await _userManager.ConfirmEmailAsync(user, code);
		StatusMessage = result.Succeeded ? "Gracias por confirmar tu email" : "Se produjo un error al confirmar el email";
		return Page();
	}

	public IActionResult OnPostToSignIn(string url)
	{
			return RedirectToPage($"/Signin",new { ReturnUrl = url});
	}

	public async Task<IActionResult> OnPostToResend(string url,string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		if(user!=null)
		{
			var Code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Code));
			var Exp = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
			var callback = Url.Page(
				pageName: "/ConfirmEmail",
				pageHandler: null,
				values: new
				{
					UserId=user.Id,
					Code,
					ReturnUrl=url,
					Exp = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Exp.ToString())),
				},
				protocol: Request.Scheme
				);
			await _emailSender.SendConfirmationEmail(user.Email!.ToString(), callback!);
			return RedirectToPage("/SignupConfirmation", new { email = user.Email.ToString(),ReturnUrl=url });
		}else
		{
			return RedirectToPage("/Signin");
		}
	}
}
