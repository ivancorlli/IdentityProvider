using IdentityProvider.Enum;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Entity;

public class ApplicationRole : IdentityRole
{
    public RoleTypes Type {get; private set;}
    public ApplicationRole(RoleTypes type)
    {
        Type = type;
    }
}