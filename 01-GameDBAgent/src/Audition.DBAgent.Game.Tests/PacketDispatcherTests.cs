using Audition.DBAgent.Game.Application.Contracts;
using Audition.DBAgent.Game.Application.Services;
using Audition.DBAgent.Game.Host.Network;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Audition.DBAgent.Game.Tests;

public class PacketDispatcherTests
{
    private readonly IGameDbAgentService _service = Substitute.For<IGameDbAgentService>();
    private readonly ILogger<PacketDispatcher> _logger = Substitute.For<ILogger<PacketDispatcher>>();
    private readonly ILegacyServerInfoProvider _legacyServerInfoProvider = Substitute.For<ILegacyServerInfoProvider>();

    [Fact]
    public async Task DispatchAsync_RawPurchasePacket_CallsPurchaseService()
    {
        var sut = new PacketDispatcher(_service, _logger);

        byte[] packet =
        [
            0,
            0xE9, 0x03, 0x00, 0x00,
            0x89, 0x13, 0x00, 0x00,
            0x1E, 0x00, 0x00, 0x00,
        ];

        var responses = await sut.DispatchAsync(packet, CancellationToken.None);

        await _service.Received(1).PurchaseItemAsync(
            Arg.Is<PurchaseCommand>(command =>
                command.UserSN == 1001 &&
                command.ItemId == 5001 &&
                command.Days == 30 &&
                command.Cost == 1000),
            Arg.Any<CancellationToken>());

        Assert.Empty(responses);
    }

    [Fact]
    public async Task DispatchAsync_BracketLegacyItemNotification_DoesNotCallApplicationService()
    {
        var sut = new PacketDispatcher(_service, _logger);

        byte[] packet =
        [
            (byte)'[',
            0x20,
            0xE9, 0x03, 0x00, 0x00,
            0x89, 0x13, 0x00, 0x00,
        ];

        var responses = await sut.DispatchAsync(packet, CancellationToken.None);

        await _service.DidNotReceiveWithAnyArgs().PurchaseItemAsync(default!, default);
        await _service.DidNotReceiveWithAnyArgs().UpdateGameResultsAsync(default!, default);
        Assert.Empty(responses);
    }

    [Fact]
    public async Task DispatchAsync_LegacyLevelQuestSync_ReturnsFramedQuestPackets()
    {
        var sut = new PacketDispatcher(_service, _logger);
        _service.GetCachedLevelQuests().Returns([
            new Domain.Entities.LevelQuest(1, 100, 80, 5, 2, 1, 1001, 1, 50, 200, 100)
        ]);

        var responses = await sut.DispatchAsync([0x5D, 0x00], CancellationToken.None);

        Assert.Equal(2, responses.Count);
        Assert.Equal(37, BitConverter.ToUInt16(responses[0], 0));
        Assert.Equal(0x5D, responses[0][2]);
        Assert.Equal(0x00, responses[0][3]);
        Assert.Equal(0x00, responses[0][4]);
        Assert.Equal(1, responses[0][5]);
        Assert.Equal(100, BitConverter.ToInt32(responses[0], 9));
        Assert.Equal(80, BitConverter.ToInt32(responses[0], 13));
        Assert.Equal(5, responses[0][17]);
        Assert.Equal(2, responses[0][18]);
        Assert.Equal(1, responses[0][19]);
        Assert.Equal((ushort)1001, BitConverter.ToUInt16(responses[0], 21));
        Assert.Equal(1, responses[0][23]);
        Assert.Equal(50, BitConverter.ToInt32(responses[0], 25));
        Assert.Equal(200, BitConverter.ToInt32(responses[0], 29));
        Assert.Equal(100, BitConverter.ToInt32(responses[0], 33));
        Assert.Equal(7, BitConverter.ToUInt16(responses[1], 0));
        Assert.Equal(0x5D, responses[1][2]);
        Assert.Equal(0x00, responses[1][3]);
        Assert.Equal(0x01, responses[1][4]);
        Assert.Equal((ushort)0, BitConverter.ToUInt16(responses[1], 5));
    }

    [Fact]
    public async Task DispatchAsync_LegacyServerInfoBootstrap_ReturnsFramedServerPacket()
    {
        var sut = new PacketDispatcher(_service, _logger, _legacyServerInfoProvider);
        _legacyServerInfoProvider
            .GetServerInfoAsync(101, Arg.Any<CancellationToken>())
            .Returns(new LegacyServerInfoData(101, "RZ", "127.0.0.1", 25511, 0, 2000, 0, 0, 1));

        var responses = await sut.DispatchAsync([0x4B, 0x00, 0x65, 0x00], CancellationToken.None);

        Assert.Single(responses);
        Assert.Equal(29, BitConverter.ToUInt16(responses[0], 0));
        Assert.Equal(0x4B, responses[0][2]);
        Assert.Equal(0x00, responses[0][3]);
        Assert.Equal(2, responses[0][4]);
        Assert.Equal((byte)'R', responses[0][5]);
        Assert.Equal((byte)'Z', responses[0][6]);
        Assert.Equal((ushort)25511, BitConverter.ToUInt16(responses[0], 7));
        Assert.Equal((ushort)0, BitConverter.ToUInt16(responses[0], 9));
        Assert.Equal((ushort)2000, BitConverter.ToUInt16(responses[0], 11));
        Assert.Equal((ushort)0, BitConverter.ToUInt16(responses[0], 13));
        Assert.Equal((ushort)0, BitConverter.ToUInt16(responses[0], 15));
        Assert.Equal((ushort)1, BitConverter.ToUInt16(responses[0], 17));
        Assert.Equal(9, responses[0][19]);
    }

    [Fact]
    public async Task DispatchAsync_LegacyServerInfoFollowUp_ReturnsFramedChannelPacket()
    {
        var sut = new PacketDispatcher(_service, _logger, _legacyServerInfoProvider);
        _legacyServerInfoProvider
            .GetChannelInfosAsync(101, Arg.Any<CancellationToken>())
            .Returns([
                new LegacyChannelInfoData(0, "RageZone", 200, 100, 0, 61, 0),
                new LegacyChannelInfoData(1, "RageZone", 200, 100, 0, 61, 2)
            ]);

        var responses = await sut.DispatchAsync([0x4B, 0x01, 0x65, 0x00], CancellationToken.None);

        Assert.Single(responses);
        Assert.Equal(46, BitConverter.ToUInt16(responses[0], 0));
        Assert.Equal(0x4B, responses[0][2]);
        Assert.Equal(0x01, responses[0][3]);
        Assert.Equal((ushort)2, BitConverter.ToUInt16(responses[0], 4));
        Assert.Equal(8, responses[0][6]);
        Assert.Equal((byte)'R', responses[0][7]);
        Assert.Equal((ushort)0, BitConverter.ToUInt16(responses[0], 15));
        Assert.Equal((ushort)200, BitConverter.ToUInt16(responses[0], 17));
        Assert.Equal((ushort)100, BitConverter.ToUInt16(responses[0], 19));
        Assert.Equal((ushort)61, BitConverter.ToUInt16(responses[0], 23));
        Assert.Equal((byte)0, responses[0][25]);
        Assert.Equal((byte)2, responses[0][45]);
    }
}