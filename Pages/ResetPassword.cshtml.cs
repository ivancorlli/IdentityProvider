using IdentityProvider.Entity;
using IdentityProvider.Pages.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IdentityProvider.Pages;

[ValidateAntiForgeryToken]
public class ResetPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    [BindProperty]
    public ModelResetPassword Reset { get; set; } = new ModelResetPassword();
    public string Error { get; set; } = string.Empty;
    public bool Invalid = false;
    public bool AllowBack { get; set; } = false;
    public string ReturnUrl { get; set; } = string.Empty;
    public ResetPasswordModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGet(string ue, string code, string exp, string returnUrl)
    {
        if (string.IsNullOrEmpty(ue) ||
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

        var userId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(ue));
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            Error = "Usuario inexistente.";
            return Page();
        }
        else if (!user.EmailConfirmed)
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
                Error = "No puedes acceder a este recurso.";
                Invalid = true;
                AllowBack = true;
                return Page();
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string ue, string code, string exp, string returnUrl)
    {

        if (string.IsNullOrEmpty(ue) ||
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
        var userId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(ue));
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                Error = error.Value.Errors.First().ErrorMessage;
                break;
            }
            return Page();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToPage("/Signin");
        }

        var result = await _userManager.ResetPasswordAsync(user, code, Reset.Password);
        if (result.Succeeded)
        {
            AllowBack = true;
            return Page();
        }
        else
        {
            if (result.Errors.Count() > 0)
            {
                var error = result.Errors.First();
                if (error.Code == "DuplicateUserName")
                {
                    Error = $"El correo electrónico ingresado ya ha sido registrado.";
                }
                else if (error.Code == "DuplicateEmail")
                {
                    Error = $"El correo electrónico ingresado ya ha sido registrado.";
                }
                else if (error.Code == "PasswordRequiresLower")
                {
                    Error = "La contrseña debe contener minusculas.";
                }
                else if (error.Code == "PasswordRequiresUpper")
                {
                    Error = "La contraseña debe contener mayusculas.";
                }
                else if (error.Code == "InvalidToken")
                {

                    Error = "No puedes acceder a este recurso.";
                    Invalid = true;
                    AllowBack = true;
                }
                else
                {
                    Error = error.Description;
                }
            }
            else
            {
                Error = "Se produjo un erro al actualizar contraseña.";

            }
            return Page();
        }
    }

    public IActionResult OnPostToSignIn(string url)
    {
        return RedirectToPage("/Signin", new { ReturnUrl = url });
    }
}
