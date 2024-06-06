using DibBase.ModelBase;
using DibBase.Obfuscation;

namespace DsCore.Api.Models;

public class CyclicFee : Entity, ISoftDelete, ITimeStamped
{
    public Payment? Payment { get; set; }
    [DsLong]
    public long PaymentId { get; set; }
    public TimeSpan PaymentInterval { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
