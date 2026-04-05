namespace AccountServer.Host.Contracts;

public sealed record DbAgentPayGameResultsRequest(uint UserSerial, int ExperienceGain, int DenGain);