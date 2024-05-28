using DibBase.ModelBase;
using DibBase.Obfuscation;

namespace DsCore.Api.Models;

public class Credentials : Entity
{
    public required string Password { get; set; }
    public required string Salt { get; set; }
    public required string VerificationCode { get; set; }
    [DsGuid(nameof(User))]
    public Guid UserGuid { get; set; }
    [DsLong]
    public long UserId { get; set; }
    public required User User { get; set; }
    public bool IsActivated { get; set; }
}
