using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;
using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Application.Contracts;

namespace Audition.GameServer.Infrastructure.Networking;

public sealed class AccountServerRegistrationClient : IAccountServerRegistrationClient
{
    private const int PacketLength = 116;
    private const int PayloadOffset = 4;
    private const int ServerInfoSize = 0x70;
    private static readonly Encoding StringEncoding = Encoding.ASCII;

    public async Task<AccountServerRegistrationResult> RegisterAsync(BootstrapConfiguration configuration, CancellationToken ct)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(configuration.AccountServer.Host, configuration.AccountServer.Port, ct);
            await using NetworkStream stream = client.GetStream();

            byte[] packet = BuildRegistrationPacket(configuration);
            await stream.WriteAsync(packet, ct);
            await stream.FlushAsync(ct);

            return new AccountServerRegistrationResult(
                true,
                $"Registered against {configuration.AccountServer.Host}:{configuration.AccountServer.Port} using legacy 0x65.0 packet");
        }
        catch (Exception ex)
        {
            return new AccountServerRegistrationResult(
                false,
                $"Unable to register with {configuration.AccountServer.Host}:{configuration.AccountServer.Port} ({ex.Message})");
        }
    }

    public static byte[] BuildRegistrationPacket(BootstrapConfiguration configuration)
    {
        var packet = new byte[PacketLength];
        BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(0, 2), PacketLength);
        packet[2] = 0x65;
        packet[3] = 0x00;

        Span<byte> payload = packet.AsSpan(PayloadOffset, ServerInfoSize);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.Slice(0, 2), configuration.ServerId);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.Slice(4, 4), configuration.ClusterId);
        WriteFixedString(payload.Slice(8, 48), configuration.ServerName);
        payload[56] = checked((byte)Math.Min(configuration.Grade, byte.MaxValue));
        BinaryPrimitives.WriteUInt32LittleEndian(payload.Slice(60, 4), 0u);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.Slice(64, 4), configuration.MaxUserCount);
        WriteFixedString(payload.Slice(68, 32), configuration.BindAddress);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.Slice(100, 2), checked((ushort)configuration.BindPort));
        BinaryPrimitives.WriteUInt32LittleEndian(payload.Slice(104, 4), configuration.Version);
        payload[108] = (byte)configuration.Status;

        return packet;
    }

    private static void WriteFixedString(Span<byte> destination, string value)
    {
        destination.Clear();
        byte[] bytes = StringEncoding.GetBytes(value ?? string.Empty);
        bytes.AsSpan(0, Math.Min(bytes.Length, destination.Length)).CopyTo(destination);
    }
}