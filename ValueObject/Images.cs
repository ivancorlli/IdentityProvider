namespace IdentityProvider.ValueObject;

public record Images
{
    public string ProfilePicture {get;private set;} = default!;
    public Images()
    {

    }
}