using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Servicio
{
    internal class Encriptado
    {
        private static readonly string AES_PASSWORD = "*Gon161AE";

        private static byte[] GetAesKey()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.ASCII.GetBytes(AES_PASSWORD));
            }
        }

        public static string EncryptAes(string message)
        {
            try
            {
                byte[] key = GetAesKey();
                byte[] iv = new byte[16];
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;

                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(aes.IV, 0, aes.IV.Length);  // Escribir IV al principio del flujo
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                            // Aplicar el relleno PKCS7 manualmente
                            int paddingLength = aes.BlockSize / 8 - messageBytes.Length % (aes.BlockSize / 8);
                            byte[] paddedMessage = new byte[messageBytes.Length + paddingLength];
                            Array.Copy(messageBytes, paddedMessage, messageBytes.Length);
                            for (int i = messageBytes.Length; i < paddedMessage.Length; i++)
                            {
                                paddedMessage[i] = (byte)paddingLength;
                            }

                            cs.Write(paddedMessage, 0, paddedMessage.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
               
                return string.Empty;
            }
        }

        public static string DecryptAes(string encryptedMessage)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);
                byte[] key = GetAesKey();
                byte[] iv = new byte[16];
                Array.Copy(encryptedBytes, iv, iv.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (MemoryStream ms = new MemoryStream(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length))
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        cs.CopyTo(decryptedStream);
                        byte[] decryptedData = decryptedStream.ToArray();

                        // Eliminar el relleno PKCS7
                        int paddingLength = decryptedData[decryptedData.Length - 1];
                        byte[] result = new byte[decryptedData.Length - paddingLength];
                        Array.Copy(decryptedData, 0, result, 0, result.Length);

                        return Encoding.UTF8.GetString(result);
                    }
                }
            }
            catch (Exception ex)
            {

                return string.Empty;
            }
        }
    }
}
