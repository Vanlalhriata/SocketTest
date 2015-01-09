using SocketCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient
{
    public class SocketClientManager
    {

        private ILogger logger;
        private Socket mainSocket;

        public SocketClientManager(ILogger pLogger)
        {
            logger = pLogger;
        }

        public void ConnectToServer()
        {
            IPEndPoint serverIp = GetServerIp();

            try
            {
                mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mainSocket.Connect(serverIp);
                logger.Log("Connected to " + serverIp.Address.ToString() + ":" + serverIp.Port + ".");
            }
            catch (SocketException ex)
            {
                logger.Log(ex.Message);
                throw ex;
            }
        }

        public void Disconnect()
        {
            if (null != mainSocket)
                mainSocket.Close();

            logger.Log("Disconnected");
        }

        private IPEndPoint GetServerIp()
        {
            // TODO: Make server ip constructor parameter
            IPHostEntry hostEntry = Dns.GetHostEntry("");
            IPAddress localIp = hostEntry.AddressList[1];
            return new IPEndPoint(localIp, 8095);
        }

    }
}
