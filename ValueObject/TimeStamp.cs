namespace IdentityProvider.ValueObject;

public record TimeStamp
{
    public long CreatedAt {get;private set;}
    public long UpdatedAt {get;private set;}

    internal TimeStamp()
    {
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        UpdatedAt = CreatedAt;
    }
    
    public void Update()
    {
        UpdatedAt =  DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

}