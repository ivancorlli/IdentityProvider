namespace IdentityProvider.ValueObject;

public class ContactPhone
{
    public long Number { get; private set; } = default!;
    public int AreaCode { get; private set; } = default!;
    public string CountryCode {get;private set;} = default!;
    public string CountryPrefix { get; private set; } =default!;

    private ContactPhone() { }
    public ContactPhone(int areaCode, long number)
    {
        Number = number;
        AreaCode = areaCode;
    }
    public ContactPhone(int areaCode, long number, string country)
    {
        Number = number;
        AreaCode = areaCode;
        CountryPrefix = country.ToUpper();
    }
}