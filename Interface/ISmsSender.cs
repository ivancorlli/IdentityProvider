namespace IdentityProvider.Interface;

public interface ISmsSender
{
    public Task TwoFactorMessage(string Email,string Code);
}