using DibBase.ModelBase;

namespace DsIdentity.Models;

public class Follow : Entity, IAudited, ISoftDelete
{
    public required User Follower { get; set; }
    public required long FollowerId { get; set; }
    public required User Followed {get; set; }
    public required long FollowedId { get; set; }
    public required string Surname { get; set; }
    public bool IsDeleted { get; set; }

    public List<string> GetFieldsToAudit() => [nameof(IsDeleted)];
}
