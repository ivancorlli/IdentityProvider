using IdentityProvider.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace IdentityProvider.Endpoints.OAuth;

public static class Token
{
	public static async Task<IResult> Handle(HttpRequest req, DevKeys keys, IDataProtectionProvider data)
	{
		var bodyBytes = await req.BodyReader.ReadAsync();
		var bodyContent = Encoding.UTF8.GetString(bodyBytes.Buffer);
		string grantTypes = "", code = "", redirectUri = "", codeVerifier = "";
		foreach (var part in bodyContent.Split("&"))
		{
			var subParts = part.Split("=");
			var key = subParts[0];
			var value = subParts[1];
			if (key == "grant_type") grantTypes = value;
			else if (key == "code") code = value;
			else if (key == "redirect_uri") redirectUri = value;
			else if (key == "code_verifier") codeVerifier = value;
		}


		if (grantTypes == "client_credentials")
		{
			var accessToken = JsonHandler(keys);
			return Results.Ok(
				new
				{
					access_token = accessToken,
				}
				);
		}



		var authCode = CodeHandler(data, code);
		if (authCode != null)
		{
			if (ValidateCode(authCode, codeVerifier))
			{
				var accessToken = JsonHandler(keys);
				return Results.Ok(
					new
					{
						access_token = accessToken,
					}
					);

			}
			else
			{
				return Results.BadRequest("Error in code verifier");
			}

		}
		else
		{
			return Results.BadRequest("Error in authcode");
		}

	}

	private static string JsonHandler(DevKeys keys)
	{
		var handler = new JsonWebTokenHandler();
		var descriptor = new SecurityTokenDescriptor()
		{
			Claims = new Dictionary<string, object>()
			{
				[JwtRegisteredClaimNames.Sub] = Guid.NewGuid().ToString(),
				["Usuario"] = "RandomUser"
			},
			Expires = DateTime.UtcNow.AddMinutes(20),
			TokenType = "Bearer",
			SigningCredentials = new SigningCredentials(keys.RsaSecurityKey, SecurityAlgorithms.RsaSha256)
		};
		var accesToken = handler.CreateToken(descriptor);
		return accesToken;
	}

	private static AuthCode? CodeHandler(IDataProtectionProvider dataProvider, string code)
	{
		var protector = dataProvider.CreateProtector("oauth");
		var codeString = protector.Unprotect(code);
		var auth = JsonSerializer.Deserialize<AuthCode>(codeString);
		return auth;
	}

	private static bool ValidateCode(AuthCode code, string codeVerifier)
	{
		var codeChallenge = Base64UrlTextEncoder.Encode(SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier)));
		return codeChallenge == code.CodeChallenge;
	}
}
