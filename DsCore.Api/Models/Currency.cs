using DibBase.ModelBase;

namespace DsCore.Api.Models;

public class Currency : Entity, INamed
{
    public required string Name { get; set; }
}
