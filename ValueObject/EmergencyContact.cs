using IdentityProvider.Enumerables;

namespace IdentityProvider.ValueObject;

public record EmergencyContact
{
    public PersonName Name { get; init; } = default!;
    public string RelationShip { get; init; } = default!;
    public ContactPhone Phone { get; init; } = default!;


    private EmergencyContact() { }
    internal EmergencyContact(PersonName name, string relationShip, ContactPhone phone)
    {
        Name = name;
        RelationShip = relationShip;
        Phone = phone;
    }
}