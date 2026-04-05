namespace AccountServer.Host.Contracts;

public sealed record DbAgentPayOperationResult(
    bool Sent,
    string Operation,
    int Opcode,
    int PacketSize,
    string Endpoint);