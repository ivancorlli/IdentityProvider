using System.Text.RegularExpressions;
using IdentityProvider.Helper;

namespace IdentityProvider.ValueObject;

public record Address
{

    public const int MaxLength = 15;
    public const int MinLength = 3;
    public const int StateMaxLength = 25;
    public static readonly Regex Reg = new("^[a-zA-Z ]+$");

    public string Country { get; init; } = default!;
    public string City { get; init; } = default!;
    public string State { get; init; } = default!;
    public string ZipCode { get; init; } = default!;
    public string? Street { get; private set; }
    public int? StreetNumber { get; private set; }

    private Address() { }
    public Address(string country, string city, string state, string zipCode)
    {
        Country = Capitalize.Create(country);
        City = Capitalize.Create(city);
        State = Capitalize.Create(state);
        ZipCode = zipCode;
    }

    public Address(string country, string city, string state, string zipCode, string street, int streetNumber)
    {
        Country = Capitalize.Create(country);
        City = Capitalize.Create(city);
        State = Capitalize.Create(state);
        ZipCode = zipCode;
        Street = Capitalize.Create(street);
        StreetNumber = streetNumber;
    }
}