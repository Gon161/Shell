using System; // Importa el espacio de nombres System, que contiene clases como String, Exception, etc.
using System.Collections.Generic; // Permite el uso de colecciones genéricas como List, Dictionary, etc.
using System.Linq; // Importa las funciones de LINQ para consultas de colecciones.
using System.Net.Sockets; // Permite trabajar con comunicación de red utilizando Sockets.
using System.Text; // Facilita la manipulación de cadenas de texto.
using System.Threading.Tasks; // Proporciona funcionalidades para trabajar con tareas asincrónicas.
using static Shell.Servicio.Propiedades; // Importa miembros estáticos de la clase Propiedades del espacio de nombres Shell.Servicio.

namespace Shell.Servicio // Definición del espacio de nombres "Shell.Servicio"
{
    public class AdminModulo // Definición de la clase pública "AdminModulo"
    {
        // Método estático que procesa una solicitud, recibe datos y un parámetro tipo Sockets
        public static string ProcesarPeticion(string sDatos, Sockets Modulo)
        {
            // Inicialización de la variable para almacenar el resultado del método
            string sResultado_del_Metodo = string.Empty;

            // Inicialización de la variable para almacenar el texto descifrado
            string sDescifrar = String.Empty;

            try
            {
                // Intenta descifrar los datos recibidos utilizando el método Desencrypt_AES de la clase Encriptado
                sDescifrar = Encriptado.Desencrypt_AES(sDatos);

                // Comienza un bloque de control switch que maneja diferentes tipos de módulos
                switch (Modulo)
                {
                    // Se comenta un caso posible para el módulo "ddlConsolidacion", sin acción asociada
                    //case Sockets.ddlConsolidacion:
                    //    break;

                    // Caso por defecto que se ejecuta si no se encuentra ningún caso específico en el switch
                    default:
                        // Si no se encuentra un caso específico, asigna un mensaje al resultado
                        sResultado_del_Metodo = "Case Default";
                        break;
                }

                // Cifra el resultado del método utilizando el método Encrypt_AES de la clase Encriptado
                string sCifrar = Encriptado.Encrypt_AES(sResultado_del_Metodo);

                // Devuelve el resultado cifrado
                return sCifrar;

            }
            catch (Exception ex) // Si ocurre una excepción, se captura y maneja aquí
            {
                // Registra el mensaje de la excepción en un archivo de log
                Log.Instancia.LogWrite(string.Format("srv_AdminModelu.ProcesarPeticion: {0} | {1}", ex.Message, ex.Source));

                // Retorna un string vacío en caso de error
                return string.Empty;
            }
        }
    }
}
