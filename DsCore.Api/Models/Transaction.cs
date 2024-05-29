using DibBase.ModelBase;
using DibBase.Obfuscation;

namespace DsCore.Api.Models;

public class Transaction : Entity, ITimeStamped
{
    public Payment Payment { get; set; }
    [DsLong]
    public long PaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } 
}