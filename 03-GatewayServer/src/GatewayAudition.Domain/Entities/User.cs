namespace GatewayAudition.Domain.Entities;

public class User
{
    public Player Player { get; } = new();
    public Session Session { get; } = new();
    public Guid? AccountSessionId { get; set; }
    public ushort CurrentGameServerId { get; set; }
    public uint UniqueIndex { get; set; }
    public byte[] RelayContext { get; set; } = new byte[12];

    public void Initialize()
    {
        Player.Initialize();
        Session.Initialize();
        AccountSessionId = null;
        CurrentGameServerId = 0;
        UniqueIndex = 0;
        RelayContext = new byte[12];
    }
}
