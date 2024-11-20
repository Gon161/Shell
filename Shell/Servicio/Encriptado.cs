using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Servicio
{
    public static class Encriptado
    {
        private static Aes oAES = null;

        private const string AES_PASSWORD = "/Gon161*IS";

        private static ASCIIEncoding oAscii = null;
        private static UTF8Encoding oUtf8 = null;
       
        private static Aes ALGO_AES
        {
            get
            {
                if (oAES == null)
                {
                    oAES = Aes.Create("AES");
                    oAES.Mode = CipherMode.CBC;
                    oAES.KeySize = 256;
                    oAES.Padding = PaddingMode.PKCS7;
                    oAES.Key = GetAesKey();
                   
                }

                return oAES;
            }
        }


        private static ASCIIEncoding ASCII
        {
            get
            {
                if (oAscii == null)
                    oAscii = new ASCIIEncoding();

                return oAscii;
            }
        }


        private static UTF8Encoding UTF8
        {
            get
            {
                if (oUtf8 == null)
                    oUtf8 = new UTF8Encoding(false);

                return oUtf8;
            }
        }


        private static byte[] GetAesKey()
        {
            byte[] yPassw = ASCII.GetBytes(AES_PASSWORD);
            using (SHA256 ALGO_SHA256 = SHA256.Create("SHA256"))
                return ALGO_SHA256.ComputeHash(yPassw);
        }

        public static string Encrypt_AES(string sMensaje)
        {
            byte[] yEncriptado = null;
            byte[] yIV = new byte[16];

            try
            {
                ALGO_AES.GenerateIV();
                yIV = ALGO_AES.IV;

                ICryptoTransform oEncryp = ALGO_AES.CreateEncryptor(ALGO_AES.Key, yIV);

                using (MemoryStream oMemoStr = new MemoryStream())
                {
                    using (CryptoStream oCryptoStr = new CryptoStream(oMemoStr, oEncryp, CryptoStreamMode.Write))
                    {
                        using (StreamWriter oWriter = new StreamWriter(oCryptoStr, UTF8))
                        {
                            oMemoStr.Write(yIV, 0, yIV.Length);
                            oWriter.Write(sMensaje);
                        }

                        yEncriptado = oMemoStr.ToArray();
                    }
                }

                return Convert.ToBase64String(yEncriptado);
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWrite(string.Format("srv_CryptoTools.Encrypt_AES: {0} | {1} ", ex.Message, ex.StackTrace));
                throw;
            }
        }


        public static string Desencrypt_AES(string sMsjEncrypt)
        {
            string sMsjDecrypt = string.Empty;
            byte[] yDecode64 = null;

            try
            {
                yDecode64 = Convert.FromBase64String(sMsjEncrypt);

                using (MemoryStream oMemoStr = new MemoryStream(yDecode64))
                {
                   
                    byte[] yIV = new byte[16];
                    oMemoStr.Read(yIV, 0, 16);

                    ICryptoTransform oDecrypt = ALGO_AES.CreateDecryptor(ALGO_AES.Key, yIV);

                    using (CryptoStream oCryptoStr = new CryptoStream(oMemoStr, oDecrypt, CryptoStreamMode.Read))
                    {
                        using (StreamReader oReader = new StreamReader(oCryptoStr, UTF8))
                            sMsjDecrypt = oReader.ReadToEnd();
                    }
                }

                return sMsjDecrypt;
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWrite(string.Format("srv_CryptoTools.Desencrypt_AES: {0} | {1} ", ex.Message, ex.StackTrace));
                throw;
            }
        }

    }
}
