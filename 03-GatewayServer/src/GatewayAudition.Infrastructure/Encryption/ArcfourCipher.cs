using GatewayAudition.Domain.Interfaces;

namespace GatewayAudition.Infrastructure.Encryption;

public sealed class ArcfourCipher : ICipher
{
    private byte[] _state = new byte[256];
    private uint _x;
    private uint _y;
    private int _count;
    private bool _initialized;

    public void Initialize()
    {
        _state = new byte[256];
        for (int i = 0; i < 256; i++)
            _state[i] = (byte)i;
        _x = 0;
        _y = 0;
        _count = 0;
        _initialized = true;
    }

    public void SetKey(byte[] key)
    {
        if (!_initialized) Initialize();

        uint j = 0;
        for (uint i = 0; i < 256; i++)
        {
            j = (j + _state[i] + key[i % key.Length]) & 0xFF;
            (_state[i], _state[j]) = (_state[j], _state[i]);
        }
        _x = 0;
        _y = 0;
    }

    public byte[] Encrypt(byte[] data)
    {
        return Process(data);
    }

    public byte[] Decrypt(byte[] data)
    {
        return Process(data);
    }

    private byte[] Process(byte[] data)
    {
        if (!_initialized) Initialize();

        byte[] result = new byte[data.Length];
        uint x = _x;
        uint y = _y;

        for (int i = 0; i < data.Length; i++)
        {
            x = (x + 1) & 0xFF;
            y = (y + _state[x]) & 0xFF;
            (_state[x], _state[y]) = (_state[y], _state[x]);
            byte k = _state[(_state[x] + _state[y]) & 0xFF];
            result[i] = (byte)(data[i] ^ k);
        }

        _x = x;
        _y = y;
        _count++;
        return result;
    }
}
