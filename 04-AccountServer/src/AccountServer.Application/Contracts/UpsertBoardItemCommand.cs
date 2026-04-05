namespace AccountServer.Application.Contracts;

public sealed record UpsertBoardItemCommand(
    ulong BoardSerial,
    ulong UserSerial,
    string UserNickname,
    ushort DestinationServer,
    string Title,
    string Payload);