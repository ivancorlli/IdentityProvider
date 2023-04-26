using IdentityProvider.Enumerables;

namespace IdentityProvider.ValueObject;

public record Access
{
    public string PermissionId { get; private set; } = default!;
    public string ResourceId { get; private set; } = default!;
    public string ResourceType { get; private set; } = default!;
    public AccessStatus Status { get; private set; }
    public long CreatedAt { get; private set; } = default!;

    private Access(){}
    private Access(
        string permissionId,
        string resourceId
        )
    {
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Status = AccessStatus.Active;
        PermissionId = permissionId;
        ResourceId = resourceId;
        ResourceType = string.Empty;
    }


    public static Access Create(
        string permissionId,
        string resourceId)
    {
        return new Access(permissionId, resourceId);
    }

    public static Access Deactive(
        string permissionId,
        string resourceId)
    {
        var a = new Access(permissionId, resourceId);
        a.Status = AccessStatus.Inactive;
        return a;
    }
}