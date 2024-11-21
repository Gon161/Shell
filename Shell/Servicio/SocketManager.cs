using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shell.Servicio.Propiedades;

namespace Shell.Servicio
{
    public class SocketManager
    {
        public ArrayList SocketsServers { set; get; }

        public SocketManager()
        {
            this.SocketsServers = new ArrayList();
            this.SocketsServers.Add(new Task(() => SocketServer.StartServer(Sockets.Calculadora)));
            this.SocketsServers.Add(new Task(() => SocketServer.StartServer(Sockets.Expresiones)));
        }

        public void StartTaskServer()
        {
            try
            {
                if (this.SocketsServers == null)
                    return;

                foreach (Task item in this.SocketsServers)
                    item.Start();
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWrite(string.Format("SocketManager.StartTaskServer: {0} | {1}", ex.Message, ex.StackTrace));
            }

        }
    }
}
