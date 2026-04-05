namespace AccountServer.Host.Contracts;

public sealed record DbAgentPayPurchaseRequest(uint UserSerial, int ItemId, int Days);