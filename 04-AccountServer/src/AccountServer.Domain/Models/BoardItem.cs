namespace AccountServer.Domain.Models;

public sealed record BoardItem(
    ulong BoardSerial,
    ulong UserSerial,
    string UserNickname,
    ushort DestinationServer,
    string Title,
    string Payload,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);