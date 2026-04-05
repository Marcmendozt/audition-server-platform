namespace LoginDBAgent.Application.Contracts;

public sealed record AvatarEquipCommand(
    uint UserSN,
    int ItemId,
    string EquipState);
