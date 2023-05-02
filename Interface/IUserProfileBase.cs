using IdentityProvider.Enumerables;
using IdentityProvider.ValueObject;

namespace IdentityProvider.Interface;

public interface IUserProfileBase
{
    public string UserId {get;}
    public PersonName Name { get; }
    public TimeStamp TimeStamp {get;}
    public UserGender Gender { get; }
    public DateTime? Birth { get; }
    public Address? Address { get; }
    public EmergencyContact? EmergencyContact { get; }
    public Bio? Bio { get; }
    public MedicalInfo? Medical { get;}

}