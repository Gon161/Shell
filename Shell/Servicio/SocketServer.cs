using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Shell.Servicio.Propiedades;

namespace Shell.Servicio
{
    internal class StateObject
    {
        public Socket ClientSocket = null;
        public const int BufferSize = 65535;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder Message = new StringBuilder();
        public Sockets Modulo;
    }
    internal class SocketServer
    {

        private static ManualResetEvent allDone = new ManualResetEvent(false);

        public static void StartServer(Sockets oModulo)
        {
            try
            {
                IPAddress oAddress = IPAddress.Parse(Propiedades.IP_SERVER_SOCKET);
                IPEndPoint oEndPoint = new IPEndPoint(oAddress, (int)oModulo);

                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(oEndPoint);
                listener.Listen(256);

                while (true)
                {
                    allDone.Reset();
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWrite(string.Format("srv_SocketServer.StartServer: {0} | {1}", ex.Message, ex.StackTrace));
               
            }
        }
        private static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                Socket handler = listener.EndAccept(ar);
                StateObject state = new StateObject();
                state.ClientSocket = handler;
                int iPort = ((IPEndPoint)handler.LocalEndPoint).Port;
                state.Modulo = ((Sockets)Enum.Parse(typeof(Sockets), iPort.ToString()));
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWrite(string.Format("srv_SocketServer.AcceptCallback: {0} | {1}", ex.Message, ex.StackTrace));
                throw;
            }
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            Socket handler = null;
            string sContent = string.Empty;

            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                handler = state.ClientSocket;
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    state.Message.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                    sContent = state.Message.ToString();
                    if (sContent.IndexOf("<EOF>") > -1)
                    {
                        sContent = sContent.Substring(0, sContent.Length - 5);
                        string sRespuesta = AdminModulo.ProcesarPeticion(sContent, state.Modulo);
                        Send(handler, sRespuesta);
                    }
                    else
                    {
                        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWrite(string.Format("srv_SocketServer.ReadCallback: {0} | {1}", ex.Message, ex.StackTrace));
            }
        }
        private static void Send(Socket handler, string sContent)
        {
            try
            {
                byte[] yData = Encoding.ASCII.GetBytes(sContent);
                handler.BeginSend(yData, 0, yData.Length, 0, new AsyncCallback(SendCallback), handler);
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWrite(string.Format("srv_SocketServer.Send: {0} | {1}", ex.Message, ex.StackTrace));
                throw;
            }
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                using (Socket handler = (Socket)ar.AsyncState)
                {
                    handler.EndSend(ar);
                    handler.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWrite(string.Format("srv_SocketServer.SendCallback: {0} | {1}", ex.Message, ex.StackTrace));
                throw;
            }
        }

    }
}
