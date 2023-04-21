
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Web;

namespace IdentityProvider.Endpoints.OAuth;

public static class Authorize
{
    public static async Task<IResult> HandleAsync(HttpContext context)
    {
        OpenIddictRequest OpenId = context.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("Se produjo un erro al obtener el token");
        AuthenticateResult auth = await context.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (!auth.Succeeded)
        {
            IList<string> schemes = new List<string>(){
                IdentityConstants.ApplicationScheme
            };
            HttpRequest Request = context.Request;
			string redirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList());
            return Results.Challenge(
                authenticationSchemes: schemes,
                properties: new AuthenticationProperties
                {
                    RedirectUri =redirectUri
                });
        }else {

		string subject = string.Empty;
		ClaimsPrincipal principal = auth.Principal;
		if(!string.IsNullOrEmpty(principal.Identity!.Name))
		{
			var id = principal.GetClaims(ClaimTypes.NameIdentifier);
			subject = id.ToString()!;
		}else {
			var id = principal.GetClaims(ClaimTypes.NameIdentifier);
			subject = id.ToString()!;
		}
        // Create a new claims principal
        IList<Claim> claims = new List<Claim>
		{
			// 'subject' claim which is required
			new Claim(OpenIddictConstants.Claims.Subject, subject),
			new Claim("some claim", "some value").SetDestinations(OpenIddictConstants.Destinations.AccessToken)
		};

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Set requested scopes (this is not done automatically)
        claimsPrincipal.SetScopes(OpenId.GetScopes());

        // Signing in with the OpenIddict authentiction scheme trigger OpenIddict to issue a code (which can be exchanged for an access token)
        var result = Results.SignIn(
			claimsPrincipal,
			properties:null,
			OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
		}
		var redirectLink = $"{OpenId.RedirectUri}?code={OpenId.Code}&state={OpenId.State}&iss={HttpUtility.UrlEncode("http://localhost/5005")}";
		return Results.Redirect(redirectLink);
    }
}
