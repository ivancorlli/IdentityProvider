using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Entity;

public class ApplicationRole : IdentityRole
{
    public string Type {get; private set;} = default!;
    
    private ApplicationRole(){}
    public ApplicationRole(string name,string type)
    {
        Name = name.Trim();
        Type = type;
    }
}