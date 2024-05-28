using DibBase.ModelBase;

namespace DsCore.Api.Models;

public class User : Entity, IAudited, ITimeStamped, ISoftDelete
{
    public required string Alias { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public string? ProfileImage { get; set; }
    public string? BackgroundImage { get; set; }
    public DateTime LastOnline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public List<string> GetFieldsToAudit() => [nameof(Name), nameof(Surname), nameof(Email)];
}
