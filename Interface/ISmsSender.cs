namespace IdentityProvider.Interface;

public interface ISmsSender
{
    public Task TwoFactorMessage(string PhoneNumber,string Code);
    public Task PhoneConfirmation(string PhoneNumber,string Code);
}