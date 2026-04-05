namespace AccountServer.Domain.Models;

public sealed record Packet(
    string Command,
    string Payload,
    bool IsEncrypted,
    int PacketSize,
    int BufferSize);