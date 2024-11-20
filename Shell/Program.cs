using Shell.Servicio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shell
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string sVersion = "1.0a";
            string sMsjInicial = string.Format("Iniciando shell, {0}", sVersion);

            Console.WriteLine(sMsjInicial);
            Log.Instancia.LogWrite(sMsjInicial);

            try
            {
                SocketManager oAdminApp = new SocketManager();
                Thread oTh_SocketManager = new Thread(new ThreadStart(oAdminApp.StartTaskServer));
                oTh_SocketManager.Start();

                Console.WriteLine("Sockets activos: {0}", oAdminApp.SocketsServers.Count);
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWrite(string.Format("Program.Main: {0} | {1}", ex.Message, ex.Source));
                Console.WriteLine("Error: {0}", ex.Message);
            }

            Console.WriteLine("Presiona una tecla para terminar...");
            Console.ReadLine();


        }
    }
}
