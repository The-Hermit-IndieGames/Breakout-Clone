using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionUtility
{
    // 設定 128、192、256 位元金鑰
    // ( 16、24、32 字節_EN )
    private static readonly string key = "這是一個加密金鑰";

    public static string EncryptString(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.GenerateIV();
            var iv = aesAlg.IV;

            using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv))
            {
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        var encrypted = msEncrypt.ToArray();
                        var result = new byte[iv.Length + encrypted.Length];
                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);
                        return Convert.ToBase64String(result);
                    }
                }
            }
        }
    }

    public static string DecryptString(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);
        using (Aes aesAlg = Aes.Create())
        {
            var iv = new byte[aesAlg.BlockSize / 8];
            var cipher = new byte[fullCipher.Length - iv.Length];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = iv;

            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            {
                using (var msDecrypt = new MemoryStream(cipher))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
