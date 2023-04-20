using IdentityProvider.Helper;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;
using System.Web;

namespace IdentityProvider.Endpoints.OAuth;

public static class Authorize
{
	public static IResult Handle(HttpRequest request, IDataProtectionProvider protector)
	{
		request.Query.TryGetValue("response_type", out var responseType);
		request.Query.TryGetValue("client_id", out var clientId);
		request.Query.TryGetValue("code_challenge", out var code_challenge);
		request.Query.TryGetValue("code_challenge_method", out var challenge_method);
		request.Query.TryGetValue("redirect_uri", out var redirectUri);
		request.Query.TryGetValue("scope", out var scope);
		request.Query.TryGetValue("state", out var state);

		var code = CodeGenerator(protector, clientId!, code_challenge!, challenge_method!, redirectUri!);

		var redirectLink = $"{redirectUri}?code={code}&state={state}&iss={HttpUtility.UrlEncode("http://localhost/5005")}";
		return Results.Redirect(redirectLink);
	}

	private static string CodeGenerator(IDataProtectionProvider protector, string clientId, string challenge, string challengeMethod, string redirect)
	{
		var protect = protector.CreateProtector("oauth");
		var newCode = new AuthCode(
			ClientId: clientId,
			CodeChallenge: challenge,
			CodeChallengeMethod: challengeMethod,
			RedirectUri: redirect,
			Expiracy: DateTime.Now.AddMinutes(5)
			);
		var code = protect.Protect(JsonSerializer.Serialize(newCode));
		return code;
	}
}
