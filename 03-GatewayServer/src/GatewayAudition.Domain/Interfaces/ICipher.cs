namespace GatewayAudition.Domain.Interfaces;

public interface ICipher
{
    byte[] Encrypt(byte[] data);
    byte[] Decrypt(byte[] data);
    void Initialize();
}
