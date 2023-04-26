using IdentityProvider.Enumerables;
using IdentityProvider.ValueObject;

namespace IdentityProvider.Interface;

public interface IUserProfileBase
{
    public string Id {get;}
    public PersonName Name { get; }
    public UserGender Gender { get; }
    public DateTime Birth { get; }
    public TimeStamp TimeStamp {get;}
    public Address Address { get; }
    public Images? Pictures {get;}
    public EmergencyContact? EmergencyContact { get; }
    public Bio? Bio { get; }
    public MedicalInfo? Medical { get;}

}