using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Shell.Servicio.Propiedades;

namespace Shell.Servicio
{
    public class AdminModulo
    {
        public static string ProcesarPeticion(string sDatos, Sockets Modulo)
        {
            string sResultado_del_Metodo = string.Empty;
            string sDescifrar = String.Empty;
            
            try
            {
                sDescifrar = Encriptado.Desencrypt_AES(sDatos);
                

                switch (Modulo)
                {
                    //case Sockets.ddlConsolidacion:

                    //    break;

                    default:
                        sResultado_del_Metodo = "Case Default";
                        break;
                }

                string sCifrar = Encriptado.Encrypt_AES(sResultado_del_Metodo);
                return sCifrar;

            }
            catch (Exception ex)
            {
                Log.Instancia.LogWrite(string.Format("srv_AdminModelu.ProcesarPeticion: {0} | {1}", ex.Message, ex.Source));
                return string.Empty;
            }
        }

    }
}
