using System.Buffers.Binary;
using System.Net.Sockets;
using GatewayAudition.Application.DTOs;
using GatewayAudition.Domain.Entities;
using GatewayAudition.Domain.Enums;
using GatewayAudition.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GatewayAudition.Application.Services;

public sealed class PacketDispatchService : IPacketHandler
{
    private readonly LoginService _loginService;
    private readonly ServerListService _serverListService;
    private readonly IPacketManager _packetManager;
    private readonly ILogger<PacketDispatchService> _logger;

    public PacketDispatchService(
        LoginService loginService,
        ServerListService serverListService,
        IPacketManager packetManager,
        ILogger<PacketDispatchService> logger)
    {
        _loginService = loginService;
        _serverListService = serverListService;
        _packetManager = packetManager;
        _logger = logger;
    }

    public async Task<bool> HandlePacketAsync(User user, Session session, Packet packet, NetworkStream stream, CancellationToken cancellationToken)
    {
        byte commandByte = packet.GetByte();
        var command = (PacketCommand)commandByte;

        return command switch
        {
            PacketCommand.Control => HandleControlPacket(session, packet),
            PacketCommand.Login => await HandleLoginAsync(user, session, packet, stream, cancellationToken),
            PacketCommand.Reserved => true,
            PacketCommand.ServerDirectory => await HandleServerDirectoryAsync(user, session, packet, stream, cancellationToken),
            _ => HandleUnknownCommand(commandByte, session)
        };
    }

    private bool HandleControlPacket(Session session, Packet packet)
    {
        byte subOpcode = packet.GetByte();
        if (subOpcode == 0x01)
        {
            _logger.LogInformation("Client requested disconnect from {Ip}", session.SessionInfo.IpAddress);
            return false;
        }

        return true;
    }

    private bool HandleUnknownCommand(byte commandByte, Session session)
    {
        _logger.LogWarning("Unknown packet command: 0x{Command:X2} from {Ip}",
            commandByte, session.SessionInfo.IpAddress);
        return false;
    }

    private async Task<bool> HandleLoginAsync(User user, Session session, Packet packet, NetworkStream stream, CancellationToken cancellationToken)
    {
        byte region = packet.GetByte();
        if (region != 0x01)
        {
            _logger.LogWarning("Unsupported login region 0x{Region:X2} from {Ip}", region, session.SessionInfo.IpAddress);
            return false;
        }

        string userId = packet.ReadPrefixedString();
        string password = packet.ReadPrefixedString();
        string version = packet.ReadPrefixedString();

        user.RelayContext = CreateRelayContext(user);

        _logger.LogInformation("[{ServerGroup}.{ServerId}] UserID({UserId}), Version({Version}), IPAddr({Ip})",
            1, 1, userId, version, session.SessionInfo.IpAddress);

        var request = new LoginRequest
        {
            Username = userId,
            Password = password,
            ClientVersion = version,
            RequestedGameServerId = 0
        };

        var loginResponse = await _loginService.ProcessLoginAsync(user, request, cancellationToken);
        var response = _packetManager.AcquirePacket();
        if (response == null)
            return false;

        try
        {
            response.PutByte(0x01);
            response.PutByte(region);

            if (loginResponse.Success)
            {
                response.PutByte(0x00);
                response.WritePrefixedString(loginResponse.UserNickname);
                response.PutUInt32(loginResponse.UserExperience);
                response.PutUInt32(loginResponse.UserSerialNumber);
                response.PutByte(loginResponse.LoginFlag);
            }
            else
            {
                response.PutByte(0x01);
            }

            response.FinalizePlainPacket();
            _logger.LogInformation(
                "[AUDITION FLOW] LOGIN_RESPONSE | UserId={UserId} | Success={Success} | UserSN={UserSerial} | Hex={Hex}",
                userId,
                loginResponse.Success,
                loginResponse.UserSerialNumber,
                Convert.ToHexString(response.Info.Buffer.AsSpan(0, (int)response.Info.PacketSize)));

            await SendPacketAsync(session, stream, response, "LOGIN_RESPONSE", cancellationToken);
            return true;
        }
        finally
        {
            _packetManager.ReleasePacket(response);
        }
    }

    private async Task<bool> HandleServerDirectoryAsync(User user, Session session, Packet packet, NetworkStream stream, CancellationToken cancellationToken)
    {
        byte subOpcode = packet.GetByte();

        _logger.LogInformation(
            "[AUDITION FLOW] ENTRAR_SALA_GATEWAY | UserSN={UserSerial} | CurrentGameServerId={GameServerId} | SubOpcode=0x{SubOpcode:X2} | IPAddr={Ip}",
            user.Player.UserSerialNumber,
            user.CurrentGameServerId,
            subOpcode,
            session.SessionInfo.IpAddress);

        switch (subOpcode)
        {
            case 0x00:
                return await HandleServerListInClusterAsync(user, session, packet, stream, cancellationToken);
            case 0x01:
                return await HandleClusterListAsync(user, session, stream, cancellationToken);
            default:
                _logger.LogWarning("Unknown server directory sub-opcode: 0x{SubOp:X2} from {Ip}",
                    subOpcode, session.SessionInfo.IpAddress);
                return false;
        }
    }

    private async Task<bool> HandleServerListInClusterAsync(User user, Session session, Packet packet, NetworkStream stream, CancellationToken cancellationToken)
    {
        ushort clusterId = packet.GetUInt16();
        var servers = _serverListService.GetServersInCluster(clusterId);

        _logger.LogInformation(
            "[AUDITION FLOW] ENTRAR_SALA_SERVER_LIST | UserSN={UserSerial} | ClusterId={ClusterId} | ServerCount={ServerCount} | IPAddr={Ip}",
            user.Player.UserSerialNumber,
            clusterId,
            servers.Count,
            session.SessionInfo.IpAddress);

        foreach (var server in servers)
        {
            _logger.LogInformation(
                "[AUDITION FLOW] ENTRAR_SALA_SERVER_TARGET | ServerId={ServerId} | Name={ServerName} | Ip={ServerIp} | Port={ServerPort} | Status={Status} | Users={CurrentUsers}/{MaxUsers}",
                server.ServerId,
                server.ServerName,
                server.ServerIp,
                server.ServerPort,
                server.ServerStatus,
                server.CurrentUserCount,
                server.MaxUserCount);
        }

        var response = _packetManager.AcquirePacket();
        if (response == null)
            return false;

        try
        {
            response.PutByte(0x03);
            response.PutByte(0x00);
            response.PutByte(0x00);
            response.PutUInt16(checked((ushort)servers.Count));

            foreach (var server in servers)
            {
                response.PutUInt16(checked((ushort)server.ServerId));
                response.WritePrefixedString(server.ServerName);
                response.WritePrefixedString(server.ServerIp);
                response.PutUInt16(CalculateLoadRatio(server.CurrentUserCount, server.MaxUserCount));
            }

            var entryServer = servers.FirstOrDefault();
            response.PutUInt16(entryServer?.ServerPort ?? 0);
            response.WritePrefixedString(entryServer?.ServerIp ?? string.Empty);

            response.FinalizePlainPacket();
            _logger.LogInformation(
                "[AUDITION FLOW] ENTRAR_SALA_SERVER_LIST_RESPONSE | UserSN={UserSerial} | ClusterId={ClusterId} | Hex={Hex}",
                user.Player.UserSerialNumber,
                clusterId,
                Convert.ToHexString(response.Info.Buffer.AsSpan(0, (int)response.Info.PacketSize)));

            await SendPacketAsync(session, stream, response, "ENTRAR_SALA_SERVER_LIST_RESPONSE", cancellationToken);
            return true;
        }
        finally
        {
            _packetManager.ReleasePacket(response);
        }
    }

    private async Task<bool> HandleClusterListAsync(User user, Session session, NetworkStream stream, CancellationToken cancellationToken)
    {
        var clusterList = _serverListService.GetServerClusterList();

        _logger.LogInformation(
            "[AUDITION FLOW] ENTRAR_SALA_CLUSTER_LIST | UserSN={UserSerial} | ClusterCount={ClusterCount} | IPAddr={Ip}",
            user.Player.UserSerialNumber,
            clusterList.Clusters.Count,
            session.SessionInfo.IpAddress);

        var response = _packetManager.AcquirePacket();
        if (response == null)
            return false;

        try
        {
            response.PutByte(0x03);
            response.PutByte(0x01);
            response.PutByte(0x00);
            response.PutByte((byte)clusterList.Clusters.Count);

            for (int index = 0; index < clusterList.Clusters.Count; index++)
            {
                var cluster = clusterList.Clusters[index];
                ushort clusterWireId = checked((ushort)(index + 1));
                byte clusterRatio = CalculateClusterRatio(cluster.ClusterId, clusterWireId);

                response.PutUInt16(clusterWireId);
                response.WritePrefixedString(cluster.ClusterName);
                response.PutByte(clusterRatio);

                _logger.LogInformation(
                    "[AUDITION FLOW] ENTRAR_SALA_CLUSTER_ENTRY | UserSN={UserSerial} | ClusterWireId={ClusterWireId} | ClusterId={ClusterId} | Name={ClusterName} | Ratio={Ratio}",
                    user.Player.UserSerialNumber,
                    clusterWireId,
                    cluster.ClusterId,
                    cluster.ClusterName,
                    clusterRatio);
            }

            response.FinalizePlainPacket();
            _logger.LogInformation(
                "[AUDITION FLOW] ENTRAR_SALA_CLUSTER_LIST_RESPONSE | UserSN={UserSerial} | Hex={Hex}",
                user.Player.UserSerialNumber,
                Convert.ToHexString(response.Info.Buffer.AsSpan(0, (int)response.Info.PacketSize)));

            await SendPacketAsync(session, stream, response, "ENTRAR_SALA_CLUSTER_LIST_RESPONSE", cancellationToken);
            return true;
        }
        finally
        {
            _packetManager.ReleasePacket(response);
        }
    }

    private byte CalculateClusterRatio(uint clusterId, ushort clusterWireId)
    {
        uint effectiveClusterId = clusterId == 0 ? clusterWireId : clusterId;
        var servers = _serverListService.GetServersInCluster(effectiveClusterId);
        uint currentUsers = 0;
        uint maxUsers = 0;
        bool hasOnlineServer = false;

        foreach (var server in servers)
        {
            currentUsers += server.CurrentUserCount;
            maxUsers += server.MaxUserCount;

            if (server.ServerStatus == 1)
            {
                hasOnlineServer = true;
            }
        }

        ushort ratio = CalculateLoadRatio(currentUsers, maxUsers);

        if (hasOnlineServer && ratio == 0)
        {
            return 1;
        }

        return (byte)ratio;
    }

    private static ushort CalculateLoadRatio(uint currentUsers, uint maxUsers)
    {
        if (maxUsers == 0)
            return 0;

        double occupancy = (double)currentUsers / maxUsers;
        return occupancy switch
        {
            < 0.20 => 0,
            < 0.40 => 1,
            < 0.60 => 2,
            < 0.80 => 3,
            _ => 4
        };
    }

    private static byte[] CreateRelayContext(User user)
    {
        var relayContext = new byte[12];
        BinaryPrimitives.WriteUInt32LittleEndian(relayContext.AsSpan(0, 4), user.UniqueIndex);
        BinaryPrimitives.WriteUInt32LittleEndian(relayContext.AsSpan(4, 4), user.Session.SessionInfo.Socket);
        BinaryPrimitives.WriteInt32LittleEndian(relayContext.AsSpan(8, 4), Environment.TickCount);
        return relayContext;
    }

    private async Task SendPacketAsync(Session session, NetworkStream stream, Packet packet, string packetLabel, CancellationToken cancellationToken)
    {
        packet.FinalizePlainPacket();
        var plainPacket = packet.Info.Buffer.AsSpan(0, (int)packet.Info.PacketSize).ToArray();
        var frame = Packet.EncryptFrame(plainPacket);

        session.LastOutboundPacketLabel = packetLabel;
        session.LastOutboundPlainHex = Convert.ToHexString(plainPacket);
        session.LastOutboundFrameHex = Convert.ToHexString(frame);

        _logger.LogInformation(
            "[AUDITION FLOW] CLIENT_PACKET_OUT | Ip={Ip} | Session={Index} | Label={Label} | PlainHex={PlainHex} | FrameHex={FrameHex}",
            session.SessionInfo.IpAddress,
            session.UniqueIndex,
            packetLabel,
            session.LastOutboundPlainHex,
            session.LastOutboundFrameHex);

        await stream.WriteAsync(frame, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }
}
