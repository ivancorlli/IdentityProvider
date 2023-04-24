using IdentityProvider.Interface;
using IdentityProvider.ValueObject;

namespace IdentityProvider.Enum;

public class UserProfile : IUserProfileBase
{
    public string Id {get;init;}
    public PersonName Name {get; private set;}

    public UserGender Gender {get; private set;}

    public DateTime Birth {get; private set;}

    public TimeStamp TimeStamp {get;private set;}
    public Images? Pictures {get;private set;}
    public EmergencyContact? EmergencyContact {get; private set;}

    public Bio? Bio {get; private set;}

    public MedicalInfo? Medical {get; private set;}


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