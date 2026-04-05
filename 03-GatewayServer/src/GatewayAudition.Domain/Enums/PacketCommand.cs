namespace GatewayAudition.Domain.Enums;

/// <summary>
/// Opcodes reales del protocolo Audition v17.
/// El cliente envía estos valores como primer byte del payload.
/// </summary>
public enum PacketCommand : byte
{
    /// <summary>Opcode 0: control packet; sub 0x01 cierra la sesion.</summary>
    Control = 0x00,

    /// <summary>Opcode 1: Login request del cliente → Gateway.</summary>
    Login = 0x01,

    /// <summary>Opcode 2: reservado/no-op en el Gateway nativo.</summary>
    Reserved = 0x02,

    /// <summary>Opcode 3: Server directory requests (sub-opcodes 0x00, 0x01, 0x02).</summary>
    ServerDirectory = 0x03,
}
