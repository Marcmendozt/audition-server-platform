using System.Net;
using System.Net.Sockets;
using System.Text;

ProxyOptions options = ProxyOptions.Parse(args);
Directory.CreateDirectory(Path.GetDirectoryName(options.LogPath)!);

using var logWriter = new TraceLogWriter(options.LogPath);
using var listener = new TcpListener(IPAddress.Parse(options.ListenHost), options.ListenPort);

listener.Start();
logWriter.WriteInfo($"Listening on {options.ListenHost}:{options.ListenPort} and forwarding to {options.TargetHost}:{options.TargetPort}");

while (true)
{
    TcpClient inboundClient = await listener.AcceptTcpClientAsync();
    int sessionId = Environment.TickCount & int.MaxValue;

    _ = Task.Run(async () =>
    {
        using var client = inboundClient;
        using var outboundClient = new TcpClient();

        string remoteEndPoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
        logWriter.WriteInfo($"SESSION {sessionId} CONNECT client={remoteEndPoint}");

        try
        {
            await outboundClient.ConnectAsync(options.TargetHost, options.TargetPort);
            logWriter.WriteInfo($"SESSION {sessionId} TARGET_CONNECTED target={options.TargetHost}:{options.TargetPort}");

            using NetworkStream inboundStream = client.GetStream();
            using NetworkStream outboundStream = outboundClient.GetStream();

            Task inboundToOutbound = PumpAsync(
                inboundStream,
                outboundStream,
                sessionId,
                "C2S",
                options,
                logWriter);

            Task outboundToInbound = PumpAsync(
                outboundStream,
                inboundStream,
                sessionId,
                "S2C",
                options,
                logWriter);

            await Task.WhenAny(inboundToOutbound, outboundToInbound);
        }
        catch (Exception ex)
        {
            logWriter.WriteError($"SESSION {sessionId} ERROR {ex}");
        }
        finally
        {
            logWriter.WriteInfo($"SESSION {sessionId} DISCONNECT");
        }
    });
}

static async Task PumpAsync(
    NetworkStream source,
    NetworkStream destination,
    int sessionId,
    string direction,
    ProxyOptions options,
    TraceLogWriter logWriter)
{
    byte[] buffer = new byte[8192];
    int chunkIndex = 0;

    while (true)
    {
        int bytesRead = await source.ReadAsync(buffer);
        if (bytesRead == 0)
        {
            return;
        }

        chunkIndex++;
        await destination.WriteAsync(buffer.AsMemory(0, bytesRead));
        await destination.FlushAsync();

        int lengthToLog = Math.Min(bytesRead, options.MaxBytesPerChunk);
        string hex = Convert.ToHexString(buffer.AsSpan(0, lengthToLog));
        string suffix = bytesRead > options.MaxBytesPerChunk
            ? $"...(+{bytesRead - options.MaxBytesPerChunk} bytes)"
            : string.Empty;

        logWriter.WriteInfo(
            $"SESSION {sessionId} {direction} CHUNK {chunkIndex} BYTES={bytesRead} HEX={hex}{suffix}");
    }
}

internal sealed record ProxyOptions(
    string ListenHost,
    int ListenPort,
    string TargetHost,
    int TargetPort,
    string LogPath,
    int MaxBytesPerChunk)
{
    public static ProxyOptions Parse(string[] args)
    {
        string listenHost = "127.0.0.1";
        int listenPort = 10700;
        string targetHost = "127.0.0.1";
        int targetPort = 10701;
        string logPath = Path.Combine(AppContext.BaseDirectory, "gateway-trace.log");
        int maxBytesPerChunk = 256;

        for (int index = 0; index < args.Length; index++)
        {
            string value = args[index];
            if (index + 1 >= args.Length)
            {
                break;
            }

            string next = args[index + 1];
            switch (value)
            {
                case "--listen-host":
                    listenHost = next;
                    index++;
                    break;
                case "--listen-port":
                    listenPort = int.Parse(next);
                    index++;
                    break;
                case "--target-host":
                    targetHost = next;
                    index++;
                    break;
                case "--target-port":
                    targetPort = int.Parse(next);
                    index++;
                    break;
                case "--log":
                    logPath = Path.GetFullPath(next);
                    index++;
                    break;
                case "--max-bytes":
                    maxBytesPerChunk = int.Parse(next);
                    index++;
                    break;
            }
        }

        return new ProxyOptions(listenHost, listenPort, targetHost, targetPort, logPath, maxBytesPerChunk);
    }
}

internal sealed class TraceLogWriter : IDisposable
{
    private readonly object gate = new();
    private readonly StreamWriter writer;

    public TraceLogWriter(string filePath)
    {
        writer = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8)
        {
            AutoFlush = true,
        };
    }

    public void WriteInfo(string message)
    {
        Write("INFO", message);
    }

    public void WriteError(string message)
    {
        Write("ERROR", message);
    }

    private void Write(string level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        lock (gate)
        {
            writer.Write('[');
            writer.Write(timestamp);
            writer.Write("] ");
            writer.Write(level);
            writer.Write(' ');
            writer.WriteLine(message);
        }
    }

    public void Dispose()
    {
        lock (gate)
        {
            writer.Dispose();
        }
    }
}