namespace GatewayAudition.Domain.Entities;

public class AccountServer
{
    public string ServerIp { get; set; } = string.Empty;
    public ushort ServerPort { get; set; }
    public bool IsConnected { get; set; }
    public Session Session { get; } = new();

    public void Initialize()
    {
        IsConnected = false;
        Session.Initialize();
    }
}
