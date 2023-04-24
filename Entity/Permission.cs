using IdentityProvider.Interface;
using IdentityProvider.ValueObject;

namespace IdentityProvider.Entity;

public class Permission : IPermissionBase
{
    public string Id {get;init;}

    public string Name {get;private set;}

    public TimeStamp TimeStamp {get;private set;}

    public Permission(string name)
    {
        Id = Guid.NewGuid().ToString();
        TimeStamp = new TimeStamp();
        Name = name;
    }
}