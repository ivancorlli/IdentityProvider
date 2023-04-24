using IdentityProvider.Enum;

namespace IdentityProvider.ValueObject;

public record Access
{
    public string PermissionId { get; init; }
    public string ResourceId { get; init; }
    public string ResourceType { get; init; }
    public AccessStatus Status { get; private set; }
    public long CreatedAt { get; init; }

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