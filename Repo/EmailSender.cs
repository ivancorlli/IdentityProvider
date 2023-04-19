using IdentityProvider.Interface;
using IdentityProvider.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace IdentityProvider.Repo;

public class EmailSender : IEmailSender
{
	private readonly SmtpClient _client;
	public EmailSender(IOptions<EmailerOptions> options)
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
	public Task SendConfirmationEmail(string Email, string Link)
	{
		_client.Send("noreplay@muver.com", Email,"Confirm Email", $"Please confirm your account by <a href='{Link}'>clicking here</a>.");

		return Task.CompletedTask;
	}

	public Task SendWelcome(string Email)
	{
		throw new NotImplementedException();
	}
}
