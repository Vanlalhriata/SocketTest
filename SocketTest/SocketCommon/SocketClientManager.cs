using System;
using System.Net;
using System.Net.Sockets;

namespace SocketCommon
{
    public class SocketClientManager : SocketManagerBase
    {
        public bool IsConnected { get { return mainSocket.Connected; } }

        public SocketClientManager(ILogger pLogger, Action<byte[]> pReceiveCallback)
            : base(pLogger, pReceiveCallback)
        {
        }

        public void ConnectToServer()
        {
            IPEndPoint serverIp = GetServerIp();

            try
            {
                mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mainSocket.Connect(serverIp);

                logger.Log("Connected to " + serverIp.Address.ToString() + ":" + serverIp.Port + ".");
                WaitForData(mainSocket);
            }
            catch (SocketException ex)
            {
                logger.Log(ex.Message);
                throw ex;
            }
        }

        public void Disconnect()
        {
            DisconnectMainSocket();
            logger.Log("Disconnected");
        }

        public override void Send(byte[] bytesToSend)
        {
            mainSocket.Send(bytesToSend);
        }

        private IPEndPoint GetServerIp()
        {
            // TODO: Make server ip constructor parameter
            IPHostEntry hostEntry = Dns.GetHostEntry("");
            IPAddress localIp = hostEntry.AddressList[1];
            return new IPEndPoint(localIp, PORT);
        }

        protected override void OnSocketDestroy(Socket socket)
        {
            logger.Log("Server disconnected");
            Disconnect();
        }

    }
}
