using GatewayAudition.Domain.Interfaces;
using System.Security.Cryptography;

namespace GatewayAudition.Infrastructure.Encryption;

public sealed class AesCipher : ICipher
{
    private byte[] _key = Array.Empty<byte>();
    private byte[] _iv = Array.Empty<byte>();
    private bool _initialized;

    public void Initialize()
    {
        _initialized = true;
    }

    public void SetKey(byte[] key, byte[] iv)
    {
        _key = key;
        _iv = iv;
        _initialized = true;
    }

    public byte[] Encrypt(byte[] data)
    {
        if (!_initialized) throw new InvalidOperationException("AES cipher not initialized");

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(data, 0, data.Length);
    }

    public byte[] Decrypt(byte[] data)
    {
        if (!_initialized) throw new InvalidOperationException("AES cipher not initialized");

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(data, 0, data.Length);
    }
}
