using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace SocketServer.Services
{
    public class CryptoService
    {
        public string _SecretKey { get; set; }
        public string _Vector { get; set; }
        public CryptoService(string Key,string Vector) {
            _SecretKey = Key;
            _Vector = Vector;
        }
        public string Encrypt(string message)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_SecretKey);
                aes.IV = Encoding.UTF8.GetBytes(_Vector);
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(message);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }
        public string Decrypt(string encryptedtext)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedtext);

            using (Aes aes = Aes.Create())
            {
                try
                {
                    aes.Key = Encoding.UTF8.GetBytes(_SecretKey);
                    aes.IV = Encoding.UTF8.GetBytes(_Vector);
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Mode = CipherMode.CBC;
                    //byte[] iv = new byte[16];
                    //Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                    //aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}
