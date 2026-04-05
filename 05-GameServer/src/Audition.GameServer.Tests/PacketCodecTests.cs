using System.Buffers.Binary;
using Audition.GameServer.Application.Contracts;
using Audition.GameServer.Domain.Models;
using Audition.GameServer.Infrastructure.Networking;
using Xunit;

namespace Audition.GameServer.Tests;

public sealed class PacketCodecTests
{
    [Fact]
    public void BuildRegistrationPacket_ProducesLegacy0x65Packet()
    {
        var configuration = new BootstrapConfiguration(
            101,
            "RZ Bootstrap",
            "127.0.0.1",
            25511,
            1,
            1,
            5000,
            1,
            ServerStatus.Online,
            new EndpointDefinition("DBAgent", "127.0.0.1", 25525),
            new EndpointDefinition("AccountServer", "127.0.0.1", 4502),
            new EndpointDefinition("Certify", "127.0.0.1", 28888, Required: false));

        byte[] packet = AccountServerRegistrationClient.BuildRegistrationPacket(configuration);

        Assert.Equal((ushort)116, BinaryPrimitives.ReadUInt16LittleEndian(packet.AsSpan(0, 2)));
        Assert.Equal(0x65, packet[2]);
        Assert.Equal(0x00, packet[3]);
        Assert.Equal((ushort)101, BinaryPrimitives.ReadUInt16LittleEndian(packet.AsSpan(4, 2)));
        Assert.Equal((uint)1, BinaryPrimitives.ReadUInt32LittleEndian(packet.AsSpan(8, 4)));
        Assert.Equal((ushort)25511, BinaryPrimitives.ReadUInt16LittleEndian(packet.AsSpan(104, 2)));
        Assert.Equal((byte)ServerStatus.Online, packet[112]);
    }

    [Fact]
    public void ParseServerInfo_ParsesLegacyBootstrapPayload()
    {
        byte[] payload =
        [
            0x4B, 0x00,
            0x02, (byte)'R', (byte)'Z',
            0xA7, 0x63,
            0x00, 0x00,
            0x88, 0x13,
            0x01, 0x00,
            0x00, 0x00,
            0x01, 0x00,
            0x09,
            (byte)'1', (byte)'2', (byte)'7', (byte)'.', (byte)'0', (byte)'.', (byte)'0', (byte)'.', (byte)'1'
        ];

        LegacyServerInfo result = GameDbAgentBootstrapClient.ParseServerInfo(payload);

        Assert.Equal("RZ", result.Name);
        Assert.Equal("127.0.0.1", result.IpAddress);
        Assert.Equal((ushort)25511, result.Port);
        Assert.Equal((ushort)5000, result.MaxUsers);
        Assert.Equal((ushort)1, result.DoubleDen);
    }

    [Fact]
    public void ParseChannelInfo_ParsesLegacyChannelPayload()
    {
        byte[] payload =
        [
            0x4B, 0x01,
            0x02, 0x00,
            0x02, (byte)'A', (byte)'1',
            0x00, 0x00,
            0xC8, 0x00,
            0x64, 0x00,
            0x00, 0x00,
            0x3D, 0x00,
            0x00,
            0x02, (byte)'A', (byte)'2',
            0x01, 0x00,
            0xC8, 0x00,
            0x64, 0x00,
            0x00, 0x00,
            0x3D, 0x00,
            0x02,
        ];

        IReadOnlyList<LegacyChannelInfo> result = GameDbAgentBootstrapClient.ParseChannelInfo(payload);

        Assert.Equal(2, result.Count);
        Assert.Equal((ushort)0, result[0].Number);
        Assert.Equal("A1", result[0].Name);
        Assert.Equal((byte)2, result[1].EventNumber);
    }
}