using IdentityProvider.Enumerables;
using IdentityProvider.Interface;
using IdentityProvider.ValueObject;

namespace IdentityProvider.Entity;

public class UserProfile : IUserProfileBase
{
    public string Id {get;private set;} = default!;
    public PersonName Name {get; private set;} = default!;
    public UserGender Gender {get; private set;} 
    public DateTime Birth {get; private set;} 
    public Address Address { get; private set; } = default!;
    public TimeStamp TimeStamp {get;private set;} = default!;
    public Images? Pictures {get;private set;} 
    public EmergencyContact? EmergencyContact {get; private set;}
    public Bio? Bio {get; private set;}
    public MedicalInfo? Medical {get; private set;}


    private UserProfile(){}
    internal UserProfile(
        PersonName name,
        UserGender gender,
        DateTime birth
        )
        {
            Id = Guid.NewGuid().ToString();
            TimeStamp = new TimeStamp();
            Name=name;
            Gender=gender;
            Birth=birth;
        }
    
    

}