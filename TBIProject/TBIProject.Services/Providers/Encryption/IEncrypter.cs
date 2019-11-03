namespace TBIProject.Services.Providers.Encryption
{
    public interface IEncrypter
    {
        string Decrypt(string cipherText);
        string Encrypt(string plainText);
    }
}