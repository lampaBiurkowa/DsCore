using DibBase.ModelBase;
using DibBase.Obfuscation;

namespace DsCore.Api.Models;

public class Payment : Entity
{
    [DsGuid(nameof(User))]
    public Guid UserGuid { get; set; }
    [DsLong]
    public long UserId { get; set; }
    public User? User { get; set; }
    public float Value { get; set; }
    [DsGuid(nameof(Currency))]
    public Guid CurrencyGuid { get; set; }
    [DsLong]
    public long CurrencyId { get; set; }
    public Currency? Currency { get; set; }
}
