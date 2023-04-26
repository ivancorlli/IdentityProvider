namespace IdentityProvider.ValueObject;

public record PersonName
{
    public string NormalizeName {get;private set;} = default!;
    public string FirstName {get;private set;} = default!;
    public string LastName {get;private set;} = default!;

    private PersonName(){}
    public PersonName(string name,string surname)
    {
        FirstName = name.ToLower().Trim();
        LastName = surname.ToLower().Trim();
        NormalizeName = $"{FirstName.ToUpper().Trim()} {LastName.ToUpper().Trim()}";
    }
}