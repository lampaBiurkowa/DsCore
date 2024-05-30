namespace DsCore.Events;

public class ToppedUpEvent
{
    public Guid UserGuid { get; set; }
    public Guid CurrecyGuid { get; set; }
    public float Value { get; set; }
}