using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace IdentityProvider.Helper;

public class DevKeys
{
	public DevKeys(IWebHostEnvironment env)
	{
		RsaKey = RSA.Create();
		var path = Path.Combine(env.ContentRootPath, "./Key/crypto_key");
		if (File.Exists(path))
		{
			var rsakey = RSA.Create();
			rsakey.ImportRSAPrivateKey(File.ReadAllBytes(path), out _);
		}
		else
		{
			var privateKey = RsaKey.ExportRSAPrivateKey();
			File.WriteAllBytes(path, privateKey);
		}
	}

	public RSA RsaKey { get; }
	public RsaSecurityKey RsaSecurityKey => new(RsaKey);
}
