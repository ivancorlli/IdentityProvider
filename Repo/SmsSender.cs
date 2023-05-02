using System.Net;
using System.Net.Mail;
using IdentityProvider.Interface;
using IdentityProvider.Options;
using Microsoft.Extensions.Options;

namespace IdentityProvider.Repo;

public class SmsSender : ISmsSender
{
    private readonly SmtpClient _client;
	public SmsSender(IOptions<EmailerOptions> options)
	{
		var host = options.Value.Host;
		var port = options.Value.Port;
		var username = options.Value.Username; 
		var password = options.Value.Password;
		var client = new SmtpClient(host, port)
		{
			Credentials = new NetworkCredential(username, password),
			EnableSsl = true
		};
		_client = client;
    }

    public Task PhoneConfirmation(string PhoneNumber, string Code)
    {
        _client.Send("noreplay@muver.com", PhoneNumber,"VerifyPhone", $"{Code}");

		return Task.CompletedTask;
    }

    public Task TwoFactorMessage(string PhoneNumber,string Code)
    {
        _client.Send("noreplay@muver.com", PhoneNumber,"Two Factor", $"{Code}");

		return Task.CompletedTask;
    }
}