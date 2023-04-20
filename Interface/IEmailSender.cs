namespace IdentityProvider.Interface;

public interface IEmailSender
{
    public Task SendWelcome(string Email);
    public Task SendConfirmationEmail(string Email, string Link);
    public Task SendResetPassword(string Email,string Link);
}
