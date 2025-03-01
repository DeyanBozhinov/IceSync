using System.Security.Cryptography;
using System.Text;

namespace IceSync.Services.Helpers;

public class EncryptionHelper
{
    private readonly string _key;

    public EncryptionHelper(string encryptionKey)
    {
        _key = encryptionKey;
    }

    public string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(_key);
            aes.GenerateIV();
            var iv = aes.IV;

            using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
            using (var ms = new MemoryStream())
            {
                ms.Write(iv, 0, iv.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public string Decrypt(string encryptedText)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(_key);

            byte[] iv = new byte[aes.BlockSize / 8];
            Array.Copy(encryptedBytes, iv, iv.Length);

            using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
            using (var ms = new MemoryStream(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
