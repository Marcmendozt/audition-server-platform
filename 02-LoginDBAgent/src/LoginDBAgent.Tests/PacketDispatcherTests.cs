using LoginDBAgent.Application.Contracts;
using LoginDBAgent.Application.Services;
using LoginDBAgent.Host.Network;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace LoginDBAgent.Tests;

public class PacketDispatcherTests
{
    private readonly ILoginDbAgentService _service = Substitute.For<ILoginDbAgentService>();
    private readonly ILogger<PacketDispatcher> _logger = Substitute.For<ILogger<PacketDispatcher>>();

    [Fact]
    public async Task DispatchAsync_LogoutVersion0WithUserSn1_RecordsLogout()
    {
        var sut = new PacketDispatcher(_service, _logger);
        byte[] packet =
        [
            0x11, 0x00,
            0x5F, 0x00,
            0x01, 0x00, 0x00, 0x00,
            0x00, 0x00,
            0x00,
            0x05,
            (byte)'a', (byte)'d', (byte)'m', (byte)'i', (byte)'n'
        ];

        await sut.DispatchAsync(packet, CancellationToken.None);

        await _service.Received(1).RecordLogoutAsync(1, "admin", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DispatchAsync_LoginVersion0_UsesLowByteOpcodeForRouting()
    {
        var sut = new PacketDispatcher(_service, _logger);
        _service.ValidateLoginAsync(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .Returns(new LoginResult(true, 1, "admin", 1000000, null));

        byte[] packet =
        [
            0x1D, 0x00,
            0x60, 0x00,
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06,
            0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C,
            0x05,
            (byte)'a', (byte)'d', (byte)'m', (byte)'i', (byte)'n',
            0x06,
            (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6'
        ];

        byte[]? response = await sut.DispatchAsync(packet, CancellationToken.None);

        Assert.NotNull(response);
        await _service.Received(1).ValidateLoginAsync(
            Arg.Is<LoginCommand>(cmd => cmd.UserId == "admin" && cmd.Password == "123456"),
            Arg.Any<CancellationToken>());
    }
}