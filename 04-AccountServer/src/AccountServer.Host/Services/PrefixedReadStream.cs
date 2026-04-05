namespace AccountServer.Host.Services;

public sealed class PrefixedReadStream(Stream innerStream, ReadOnlyMemory<byte> prefix) : Stream
{
    private readonly Stream innerStream = innerStream;
    private ReadOnlyMemory<byte> prefix = prefix;
    private int prefixOffset;

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => innerStream.CanWrite;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        innerStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (TryCopyPrefix(buffer.AsSpan(offset, count), out var copied))
        {
            return copied;
        }

        return innerStream.Read(buffer, offset, count);
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (TryCopyPrefix(buffer.Span, out var copied))
        {
            return copied;
        }

        return await innerStream.ReadAsync(buffer, cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        innerStream.Write(buffer, offset, count);
    }

    private bool TryCopyPrefix(Span<byte> destination, out int copied)
    {
        if (prefixOffset >= prefix.Length)
        {
            copied = 0;
            return false;
        }

        copied = Math.Min(destination.Length, prefix.Length - prefixOffset);
        prefix.Span.Slice(prefixOffset, copied).CopyTo(destination);
        prefixOffset += copied;
        return true;
    }
}