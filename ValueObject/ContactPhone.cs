namespace IdentityProvider.ValueObject;

public class ContactPhone
{
    public string Number { get; private set; } = default!;
    public string CountryCode {get;private set;} = default!;
    public string CountryPrefix { get; private set; } =default!;

    private ContactPhone() { }
    public ContactPhone(string countryCode, string number, string country)
    {
        Number = number;
        CountryCode = countryCode;
        CountryPrefix = country.ToUpper();
    }
}