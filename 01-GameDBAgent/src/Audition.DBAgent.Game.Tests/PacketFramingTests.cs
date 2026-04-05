using Audition.DBAgent.Game.Host.Network;
using Xunit;

namespace Audition.DBAgent.Game.Tests;

public sealed class PacketFramingTests
{
    [Fact]
    public void UsesLengthPrefix_ReturnsFalse_ForLegacyLevelQuestRawRequest()
    {
        Assert.False(PacketFraming.UsesLengthPrefix([0x5D, 0x00]));
    }

    [Fact]
    public void UsesLengthPrefix_ReturnsFalse_ForLegacyServerInfoRawRequest()
    {
        Assert.False(PacketFraming.UsesLengthPrefix([0x4B, 0x00, 0x65, 0x00]));
    }

    [Fact]
    public void UsesLengthPrefix_ReturnsTrue_ForStandardFramedPacket()
    {
        Assert.True(PacketFraming.UsesLengthPrefix([0x25, 0x00, 0x5D, 0x00, 0x00]));
    }

    [Fact]
    public void UsesLengthPrefix_ReturnsTrue_ForLegacyLengthPrefixedPacket()
    {
        byte[] packet = [6, 0, (byte)'[', 0x20, 0x01, 0x00];

        bool usesLengthPrefix = PacketFraming.UsesLengthPrefix(packet);

        Assert.True(usesLengthPrefix);
    }

    [Fact]
    public void UsesLengthPrefix_ReturnsFalse_ForRawPurchasePacket()
    {
        byte[] packet =
        [
            0,
            0xE9, 0x03, 0x00, 0x00,
            0x89, 0x13, 0x00, 0x00,
            0x1E, 0x00, 0x00, 0x00,
        ];

        bool usesLengthPrefix = PacketFraming.UsesLengthPrefix(packet);

        Assert.False(usesLengthPrefix);
    }

    [Fact]
    public void TryExtractPayload_ConsumesSingleFramedPacket()
    {
        var buffer = new List<byte> { 6, 0, (byte)'[', 0x20, 0x01, 0x00 };

        bool extracted = PacketFraming.TryExtractPayload(buffer, out byte[] payload);

        Assert.True(extracted);
        Assert.Equal([(byte)'[', 0x20, 0x01, 0x00], payload);
        Assert.Empty(buffer);
    }
}