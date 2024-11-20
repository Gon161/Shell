using System; // Importa el espacio de nombres System, que contiene clases como String, Exception, etc.
using System.Collections.Generic; // Permite el uso de colecciones genéricas como List, Dictionary, etc.
using System.IO; // Permite el uso de operaciones de entrada y salida de archivos y flujos de datos.
using System.Linq; // Importa las funciones de LINQ para consultas de colecciones.
using System.Security.Cryptography; // Contiene clases relacionadas con la criptografía, como AES y SHA256.
using System.Text; // Facilita la manipulación de cadenas de texto.
using System.Threading.Tasks; // Proporciona funcionalidades para trabajar con tareas asincrónicas.

namespace Shell.Servicio // Definición del espacio de nombres "Shell.Servicio"
{
    public static class Encriptado // Definición de la clase estática "Encriptado"
    {
        private static Aes oAES = null; // Declaración de un objeto estático para AES (algoritmo de encriptación)
        private const string AES_PASSWORD = "/Gon161*IS"; // Definición de una constante para la contraseña AES
        private static ASCIIEncoding oAscii = null; // Declaración de un objeto estático para la codificación ASCII
        private static UTF8Encoding oUtf8 = null; // Declaración de un objeto estático para la codificación UTF8

        // Propiedad privada que devuelve una instancia de Aes configurada con parámetros específicos
        private static Aes ALGO_AES
        {
            get
            {
                if (oAES == null) // Si oAES es nulo (es decir, no se ha inicializado)
                {
                    oAES = Aes.Create("AES"); // Crea una instancia del algoritmo AES
                    oAES.Mode = CipherMode.CBC; // Establece el modo de cifrado CBC (Cipher Block Chaining)
                    oAES.KeySize = 256; // Establece el tamaño de la clave en 256 bits
                    oAES.Padding = PaddingMode.PKCS7; // Establece el relleno PKCS7
                    oAES.Key = GetAesKey(); // Asigna la clave de encriptación generada por el método GetAesKey()
                }
                return oAES; // Devuelve el objeto AES configurado
            }
        }

        // Propiedad privada para obtener una instancia de ASCIIEncoding, que codifica caracteres a ASCII
        private static ASCIIEncoding ASCII
        {
            get
            {
                if (oAscii == null) // Si oAscii es nulo (no ha sido inicializado)
                    oAscii = new ASCIIEncoding(); // Crea una nueva instancia de ASCIIEncoding

                return oAscii; // Devuelve la instancia de ASCIIEncoding
            }
        }

        // Propiedad privada para obtener una instancia de UTF8Encoding, que codifica caracteres a UTF-8
        private static UTF8Encoding UTF8
        {
            get
            {
                if (oUtf8 == null) // Si oUtf8 es nulo (no ha sido inicializado)
                    oUtf8 = new UTF8Encoding(false); // Crea una nueva instancia de UTF8Encoding sin BOM (Byte Order Mark)

                return oUtf8; // Devuelve la instancia de UTF8Encoding
            }
        }

        // Método privado que obtiene una clave de 256 bits para AES usando SHA256 para procesar la contraseña
        private static byte[] GetAesKey()
        {
            byte[] yPassw = ASCII.GetBytes(AES_PASSWORD); // Convierte la contraseña AES a un array de bytes usando ASCII
            using (SHA256 ALGO_SHA256 = SHA256.Create("SHA256")) // Crea una instancia del algoritmo SHA256
                return ALGO_SHA256.ComputeHash(yPassw); // Retorna el hash de la contraseña procesada por SHA256
        }

        // Método público estático que encripta un mensaje utilizando AES
        public static string Encrypt_AES(string sMensaje)
        {
            byte[] yEncriptado = null; // Variable para almacenar los datos encriptados
            byte[] yIV = new byte[16]; // Array de bytes para el vector de inicialización (IV)

            try
            {
                ALGO_AES.GenerateIV(); // Genera un nuevo vector de inicialización (IV) aleatorio
                yIV = ALGO_AES.IV; // Asigna el IV generado

                ICryptoTransform oEncryp = ALGO_AES.CreateEncryptor(ALGO_AES.Key, yIV); // Crea un objeto de encriptación utilizando la clave y el IV

                using (MemoryStream oMemoStr = new MemoryStream()) // Crea un flujo de memoria para almacenar los datos encriptados
                {
                    using (CryptoStream oCryptoStr = new CryptoStream(oMemoStr, oEncryp, CryptoStreamMode.Write)) // Crea un flujo de cifrado
                    {
                        using (StreamWriter oWriter = new StreamWriter(oCryptoStr, UTF8)) // Crea un escritor para escribir en el flujo de cifrado
                        {
                            oMemoStr.Write(yIV, 0, yIV.Length); // Escribe el IV al inicio del flujo
                            oWriter.Write(sMensaje); // Escribe el mensaje encriptado en el flujo de cifrado
                        }

                        yEncriptado = oMemoStr.ToArray(); // Convierte los datos encriptados a un array de bytes
                    }
                }

                return Convert.ToBase64String(yEncriptado); // Devuelve los datos encriptados como una cadena Base64
            }
            catch (Exception ex) // Si ocurre un error, se captura y maneja la excepción
            {
                Log.Instancia.LogWrite(string.Format("srv_CryptoTools.Encrypt_AES: {0} | {1} ", ex.Message, ex.StackTrace)); // Registra el error en un log
                throw; // Vuelve a lanzar la excepción
            }
        }

        // Método público estático que desencripta un mensaje utilizando AES
        public static string Desencrypt_AES(string sMsjEncrypt)
        {
            string sMsjDecrypt = string.Empty; // Variable para almacenar el mensaje desencriptado
            byte[] yDecode64 = null; // Array de bytes para almacenar los datos decodificados

            try
            {
                yDecode64 = Convert.FromBase64String(sMsjEncrypt); // Decodifica el mensaje encriptado de Base64 a bytes

                using (MemoryStream oMemoStr = new MemoryStream(yDecode64)) // Crea un flujo de memoria con los datos encriptados
                {
                    byte[] yIV = new byte[16]; // Array de bytes para el IV
                    oMemoStr.Read(yIV, 0, 16); // Lee el IV del flujo de memoria

                    ICryptoTransform oDecrypt = ALGO_AES.CreateDecryptor(ALGO_AES.Key, yIV); // Crea un objeto de desencriptación usando la clave y el IV

                    using (CryptoStream oCryptoStr = new CryptoStream(oMemoStr, oDecrypt, CryptoStreamMode.Read)) // Crea un flujo de desencriptado
                    {
                        using (StreamReader oReader = new StreamReader(oCryptoStr, UTF8)) // Crea un lector para leer el flujo desencriptado
                            sMsjDecrypt = oReader.ReadToEnd(); // Lee el mensaje desencriptado
                    }
                }

                return sMsjDecrypt; // Devuelve el mensaje desencriptado
            }
            catch (Exception ex) // Si ocurre un error, se captura y maneja la excepción
            {
                Log.Instancia.LogWrite(string.Format("srv_CryptoTools.Desencrypt_AES: {0} | {1} ", ex.Message, ex.StackTrace)); // Registra el error en un log
                throw; // Vuelve a lanzar la excepción
            }
        }
    }
}

