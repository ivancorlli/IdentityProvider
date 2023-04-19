using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace IdentityProvider.Endpoints;

public static class Login
{
	public static async Task<IResult> Handle(HttpContext context, string returnUrl)
	{
		await context.SignInAsync("cookie", new ClaimsPrincipal(

				new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }, "cookie")

			));

		return Results.Redirect(returnUrl);
	}
}
