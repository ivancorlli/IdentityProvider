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
	public string UE { get; set; } = string.Empty;
	public bool Resend { get;set; } =false;
	public ConfirmEmailModel(UserManager<ApplicationUser> userManager,IEmailSender sender)
	{
		_userManager = userManager;
		_emailSender = sender;
	}

	public async Task<IActionResult> OnGetAsync(string ue, string code, string returnUrl, string exp)
	{
		if (
			string.IsNullOrEmpty(ue) ||
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
		string userId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(ue));
		ApplicationUser? user = await _userManager.FindByIdAsync(userId);
		if (user == null)
		{
			return RedirectToPage("/Signin", new {ReturnUrl});
		}

		UE = ue;
		if (user.EmailConfirmed) {
			return RedirectToPage($"/Signin",new { ReturnUrl });
		}

		exp = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(exp));
		long ExpirationTime = long.Parse(exp);
		if(ExpirationTime > 0 )
		{
			long Now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			if(Now > ExpirationTime)
			{
				ErrorMessage = "Verificacion expirada.";
				if (!user.EmailConfirmed) Resend = true;
				return Page();
			}
			code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
			IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
			
			if(result.Succeeded)
			{
				StatusMessage = "Gracias por confirmar tu email.";
				
			}else {
				if(result.Errors.Count() > 0)
				{
					ErrorMessage = result.Errors.First().Description;
				}else {
					ErrorMessage = "Se produjo un error al confirmar el email";
				}
			}
			return Page();
		}else {
			ErrorMessage = "Verificacion expirada.";
			if (!user.EmailConfirmed) Resend = true;
			return Page();
		}


	}

	public IActionResult OnPostToSignIn(string url)
	{
			return RedirectToPage($"/Signin",new { ReturnUrl = url});
	}

	public async Task<IActionResult> OnPostToResend(string url,string ue)
	{	
		ReturnUrl = url;
		UE=ue;
		string userId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(ue));
		ApplicationUser? user = await _userManager.FindByIdAsync(userId);
		if(user!=null)
		{
			string Code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Code));
			long Exp = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
			string callback = Url.Page(
				pageName: "/ConfirmEmail",
				pageHandler: null,
				values: new
				{
					UE=WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id)),
					Code,
					ReturnUrl=url,
                    Exp = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Exp.ToString())),
				},
				protocol: Request.Scheme
				)!;
			await _emailSender.SendConfirmationEmail(user.Email!, callback);
			return RedirectToPage("/SignupConfirmation", new { UE=WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id)),ReturnUrl=url });
		}else
		{
			return RedirectToPage("/Signin");
		}
	}
}
