using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace IdentityProvider.Endpoints.OAuth;

public class Token
{
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    public static async Task<IResult> Exchange(HttpContext context)
    {
        OpenIddictRequest OpenId = context.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("Se produjo un erro al obtener el token");
        ClaimsPrincipal claimsPrincipal;
        if (OpenId.IsClientCredentialsGrantType())
        {
            // Note: the client credentials are automatically validated by OpenIddict:
            // if client_id or client_secret are invalid, this action won't be invoked.

            ClaimsIdentity identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Subject (sub) is a required field, we use the client id as the subject identifier here.
            identity.AddClaim(OpenIddictConstants.Claims.Subject, OpenId.ClientId ?? throw new InvalidOperationException("Se produjo un error al obtener client ID"));

            // Add some claim, don't forget to add destination otherwise it won't be added to the access token.
            identity.AddClaim("some-claim", "some-value", OpenIddictConstants.Destinations.AccessToken);

            claimsPrincipal = new ClaimsPrincipal(identity);

            claimsPrincipal.SetScopes(OpenId.GetScopes());
        }
        else if (OpenId.IsAuthorizationCodeGrantType())
        {
            AuthenticateResult auth = await context.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (auth.Succeeded)
            {
                claimsPrincipal = auth.Principal;
            }
            else
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
            }
        }
        else
        {
            throw new InvalidOperationException("El grant_type no es soportado.");
        }

        // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
        return Results.SignIn(claimsPrincipal, null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

}
