using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Servicio
{
    public class Propiedades
    {
        public static string IP_SERVER_SOCKET
        {
            get { return ConfigurationManager.AppSettings["sIPService"].ToString(); }
        }
        public enum Sockets
        {
            [Description("Port 8380, Módulo Calculadora")]
            Calculadora = 8381,
            [Description("Port 8380, Módulo Validacion")]
            Expresiones = 8381,
        }
    }
}
