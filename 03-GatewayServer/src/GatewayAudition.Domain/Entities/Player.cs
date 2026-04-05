namespace GatewayAudition.Domain.Entities;

public class Player
{
    public uint UserSerialNumber { get; set; }
    public byte CompanyId { get; set; }
    public uint Experience { get; set; }

    public void Initialize()
    {
        UserSerialNumber = 0;
        CompanyId = 0;
        Experience = 0;
    }
}
