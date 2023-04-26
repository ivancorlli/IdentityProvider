
namespace IdentityProvider.Helper;

public class Capitalize
{

    public static string Create(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            var Initial = char.ToUpper(value[0]);
            var Rest = value.Substring(1).ToLower();
            return Initial.ToString() + Rest;
        }
        else
        {
            return string.Empty;
        }
    }
}