using IdentityProvider.Interface;
using IdentityProvider.ValueObject;

namespace IdentityProvider.Entity;

public class Permission : IPermissionBase
{
    public string Id {get;private set;} = default!;

    public string Name {get;private set;} =default!;

    public TimeStamp TimeStamp {get;private set;} = default!;

    private Permission() {}
    public Permission(string name)
    {
        Id = Guid.NewGuid().ToString();
        TimeStamp = new TimeStamp();
        Name = name;
    }
}