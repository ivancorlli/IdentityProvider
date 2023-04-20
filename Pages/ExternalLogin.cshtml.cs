using IdentityProvider.Entity;
using IdentityProvider.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using IdentityProvider.Interface;

namespace IdentityProvider.Pages;

public class ExternalLogin : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserLoginStore<ApplicationUser> _userLogin;
    private readonly IOptions<ReturnUrlOptions> _defaultReturn;
    private readonly IEmailSender _emailSender;

    public string ReturnUrl { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public bool AllowBack { get; set; } = false;
    public ExternalLogin(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        IOptions<ReturnUrlOptions> returnUrl,
        IEmailSender emailSender
        )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _defaultReturn = returnUrl;
        _emailSender = emailSender;
        _userLogin = GetLogins();
    }

    public IActionResult OnGet() => RedirectToPage("/Signin");

    public IActionResult OnPost(string provider, string returnUrl)
    {
        if(string.IsNullOrEmpty(returnUrl) || string.IsNullOrEmpty(provider)) 
            return Redirect("/Signin");

        ReturnUrl = returnUrl;
        var redirectUrl = Url.Page("/ExternalLogin", pageHandler: "Callback", values: new { ReturnUrl});
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        ReturnUrl = returnUrl ?? _defaultReturn.Value.Default;
        if (remoteError != null)
        {
            Error = $"Error del provedor: {remoteError}";
            return Page();
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            Error = "Se produjo un error al obtener tus datos.";
            return Page();
        }

        // Iniciamos el uaurio si ya ha registrado este provedor previamente.
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            return Redirect(ReturnUrl);
        }
        else if (result.IsNotAllowed)
        {
            Error = "Debes verificar tu cuenta, por favor revisa tu correo electronico";
            AllowBack = true;
            return Page();
        }
        else if (result.IsLockedOut)
        {
            return RedirectToPage("/Lockout");
        }
        else
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            // Si tiene email, continuamos
            if (email != null)
            {
                return await CreateUser(ReturnUrl);
            }
            else
            {
                // Si no tiene email, mostramos error
                Error = $"Se produjo un error al obtener tus datos de {info.ProviderDisplayName}.";
                AllowBack = true;
                return Page();
            }
        }
    }

    private async Task<IActionResult> CreateUser(string returnUrl)
    {
        // Obtenemos la informacion de login del usuario
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            // Error al obtener  el provedor
            Error = "Se produjo un error al obtener tus datos.";
            return Page();
        }
        else
        {
            // Obttenemos el email de los claims
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            // Buscamos si el email ya ha sido registrado
            var user = await _userManager.FindByEmailAsync(email!);
            // Si el usuario no existe
            if (user == null)
            {
                // Creamos un nuevo usuario
                user = new ApplicationUser
                {
                    Email = email,
                    UserName = email
                };
                // Guardamos en la base
                var result = await _userManager.CreateAsync(user);
                // Si el resultado es exitoso
                if (result.Succeeded)
                {
                    // Guardamos el provedor
                    result = await _userManager.AddLoginAsync(user, info);
                    // Si el resultado es exitoso
                    if (result.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var exp = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
                        var callback = Url.Page(
                            pageName: "/ConfirmEmail",
                            pageHandler: null,
                            values: new
                            {
                                UserId=userId,
                                Code=code,
                                ReturnUrl = returnUrl,
                                Exp = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(exp.ToString())),
                            },
                            protocol: Request.Scheme
                            );
                        await _emailSender.SendWelcome(user.Email!.ToString());
                        await _emailSender.SendConfirmationEmail(user.Email!.ToString(), callback!);
                        return RedirectToPage("/SignupConfirmation", new { email = user.Email, ReturnUrl = returnUrl});
                    }
                    else
                    {
                        // Errores al agreagar proveedor
                        foreach (var error in result.Errors)
                        {
                            Error = error.Description;
                            break;
                        }
                        return Page();
                    }
                }
                else
                {
                    // Errores al crear
                    foreach (var error in result.Errors)
                    {
                        Error = error.Description;
                        break;
                    }
                    return Page();
                }
            }
            else
            {
                // Si el usuario ya ha sido registrado

                // Buscamos los proveedores
                var providers = await _userLogin.GetLoginsAsync(user, CancellationToken.None);
                var exists = providers.Where(x => x.LoginProvider == info.LoginProvider).ToList();
                if (exists.Count > 0)
                {
                    Error = $"Tu cuenta ya ha sido registrada con {exists.First().ProviderDisplayName}";
                    AllowBack = true ;
                    return Page();
                }
                else
                {
                    // Agreagamos el login
                    var result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        // Si el email esta confirmado
                        if (user.EmailConfirmed)
                        {
                            // Inciamos sesion
                            var login = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                            if (login.Succeeded)
                            {
                                return Redirect(returnUrl);
                            }
                            else if (login.IsNotAllowed)
                            {
                                Error = "Debes verificar tu cuenta, por favor revisa tu correo electronico";
                                AllowBack = true;
                                return Page();
                            }
                            else if (login.IsLockedOut)
                            {
                                return RedirectToPage("/Lockout");
                            }
                            else
                            {
                                Error = "No pudimos completar tu ingreso";
                                return Page();
                            }
                        }
                        else
                        {
                            // Si el email no esta confirmado, le enviamos una solicitud de verificacion
                            var userId = await _userManager.GetUserIdAsync(user);
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                            var exp = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
                            var callback = Url.Page(
                                pageName: "/ConfirmEmail",
                                pageHandler: null,
                                values: new
                                {
                                    UserId = userId,
                                    Code=code,
                                    ReturnUrl = returnUrl,
                                    Exp = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(exp.ToString())),
                                },
                                protocol: Request.Scheme
                                );
                            await _emailSender.SendConfirmationEmail(user.Email!.ToString(), callback!);
                            return RedirectToPage("/SignupConfirmation", new { email = user.Email, ReturnUrl=returnUrl });
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Error = error.Description;
                            break;
                        }
                        return Page();
                    }

                }
            }
        }

    }
    public IActionResult OnPostToSignIn(string url)
    {
        return RedirectToPage("/Signin", new {ReturnUrl = url });
    }
    private IUserLoginStore<ApplicationUser> GetLogins()
    {
        return (IUserLoginStore<ApplicationUser>)_userStore;
    }
}