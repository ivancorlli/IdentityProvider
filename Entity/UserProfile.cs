using IdentityProvider.Enumerables;
using IdentityProvider.Interface;
using IdentityProvider.ValueObject;

namespace IdentityProvider.Entity;

public class UserProfile : IUserProfileBase
{
    public string UserId { get; private set; } = default!;
    public PersonName Name { get; private set; } = default!;
    public TimeStamp TimeStamp { get; private set; } = default!;
    public UserGender Gender { get; private set; }
    public DateTime? Birth { get; private set; }
    public string? ProfilePicture { get; private set; } 
    public string? LandscapePicture { get; private set; }
    public Address? Address { get; private set; }
    public EmergencyContact? EmergencyContact { get; private set; }
    public MedicalInfo? Medical { get; private set; }
    public Bio? Bio { get; private set; }


    private UserProfile() { }
    internal UserProfile(string userId, PersonName name)
    {
        UserId = userId;
        TimeStamp = new TimeStamp();
        Name = name;
    }

    public UserProfile AddGender(UserGender gender)
    {
        Gender = gender;
        return this;
    }

    public void AddBirth(DateTime birth)
    {
        Birth = birth;
    }

    public void AddProfileImage(string profilePicture)
    {
        if (ProfilePicture is null)
        {
            ProfilePicture = profilePicture;
        }
    }

}