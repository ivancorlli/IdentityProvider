namespace IdentityProvider.ValueObject;

public record Images
{
    public string ProfilePicture {get;private set;} = null!;
    public string LandscapePicture {get; private set;} = null!;
    private Images(){}
    public Images(string profilePicture)
    {
        ProfilePicture = profilePicture;
    }
}