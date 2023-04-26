namespace IdentityProvider.ValueObject;

public record Bio
{
    public static int MaxLength = 300;

    public string Value { get; private set; } = default!;

    private Bio(){}
    public Bio(string value)
    {
        Value = value;
    }
}