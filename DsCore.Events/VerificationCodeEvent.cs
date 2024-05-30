namespace DsCore.Events;

public class VerificationCodeEvent
{
    public Guid UserGuid { get; set; }
    public required string VerificationCode { get; set; }
}