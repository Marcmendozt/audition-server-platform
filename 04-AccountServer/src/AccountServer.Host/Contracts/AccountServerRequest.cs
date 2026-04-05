using System.Text.Json;

namespace AccountServer.Host.Contracts;

public sealed record AccountServerRequest(string Command, JsonElement Payload);