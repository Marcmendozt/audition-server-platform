namespace GatewayAudition.Domain.ValueObjects;

public sealed class RelayContext
{
    public object? Client { get; set; }
    public int InstanceHandle { get; set; }
    public object? Parameter { get; set; }
}
