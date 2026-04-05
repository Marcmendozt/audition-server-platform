namespace AccountServer.Host.Contracts;

public sealed record PacketManagerStatus(
    int BuffersInUse,
    long BuffersRented,
    long BuffersReturned,
    long PacketsProcessed);