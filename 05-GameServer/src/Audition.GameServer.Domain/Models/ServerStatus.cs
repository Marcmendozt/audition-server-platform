namespace Audition.GameServer.Domain.Models;

public enum ServerStatus : byte
{
    Offline = 0,
    Online = 1,
    Busy = 2,
    Maintenance = 3,
}