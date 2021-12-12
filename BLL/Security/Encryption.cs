using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Security
{
    public class Encryption
    {
        public static string EncryptData(string data, string key)
        {
            var aes = new AesGcm(Convert.FromHexString(key));
            var nonce = new byte[12];
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var encryptedData = new byte[dataBytes.Length];
            var tag = new byte[16];
            RandomNumberGenerator.Fill(nonce);
            aes.Encrypt(nonce, dataBytes, encryptedData, tag);
            var result = Convert.ToHexString(encryptedData.Concat(nonce).Concat(tag).ToArray());
            return result;
        }

        public static string DecryptData(string data, string key)
        {
            var aes = new AesGcm(Convert.FromHexString(key));
            var encryptedBytes = Convert.FromHexString(data);
            var dataBytes = encryptedBytes[0 .. (encryptedBytes.Length - 12 - 16)];
            var nonce = encryptedBytes[dataBytes.Length .. (dataBytes.Length + 12)];
            var tag = encryptedBytes[(dataBytes.Length + 12) .. (dataBytes.Length + 12 + 16)];
            var decryptedBytes = new byte[dataBytes.Length];
            aes.Decrypt(nonce, dataBytes, tag, decryptedBytes);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}