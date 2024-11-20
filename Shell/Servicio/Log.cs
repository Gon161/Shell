using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Servicio
{
    public class Log
    {
        private static volatile Log oInstancia = null;
        private static readonly object objLock = new object();
        public static Log Instancia
        {
            get
            {
                if (oInstancia == null)
                {
                    lock (objLock)
                    {
                        if (oInstancia == null)
                            oInstancia = new Log();
                    }
                }
                return oInstancia;
            }
        }
        public void LogWrite(string sMsj)
        {
            try
            {
                lock (objLock)
                {
                    string sPathLogDefault = string.Format(@".\Log - {0}.txt", DateTime.Now.ToString("dd-MM-yyyy"));

                    using (StreamWriter oLogWriter = new StreamWriter(sPathLogDefault, true))
                        oLogWriter.WriteLine(string.Format("[{0}] - {1}", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), sMsj));
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
