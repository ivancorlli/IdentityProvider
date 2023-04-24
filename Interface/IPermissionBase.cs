using IdentityProvider.ValueObject;

namespace IdentityProvider.Interface;

public interface IPermissionBase
{
    public string Id {get;}
    public string Name {get;}
    public TimeStamp TimeStamp {get;}
    
}