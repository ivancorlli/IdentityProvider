using System.IO.Pipes;
using IdentityProvider.Enumerables;
using IdentityProvider.Helper;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Entity;

public class ApplicationRole : IdentityRole
{
    public RoleTypes Type {get; private set;} = default!;
    
    private ApplicationRole(){}
    public ApplicationRole(string name,RoleTypes type)
    {
        Name = name.Trim();
        Type = type;
    }
}