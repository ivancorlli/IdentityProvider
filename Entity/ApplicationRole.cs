using IdentityProvider.Enumerables;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Entity;

public class ApplicationRole : IdentityRole
{
    public RoleTypes Type {get; private set;} = default!;
    
    public ApplicationRole(){}
    public ApplicationRole(RoleTypes type)
    {
        Type = type;
    }
}