using System;
using System.Net;
using System.Net.Sockets;

namespace SocketCommon
{
    public class SocketClientManager : SocketManagerBase
    {
        private IPAddress serverIp;

        public bool IsConnected { get { return mainSocket.Connected; } }

        public SocketClientManager(ILogger pLogger, Action<byte[]> pReceiveCallback, IPAddress pServerIp)
            : base(pLogger, pReceiveCallback)
        {
            serverIp = pServerIp;
        }

        public void ConnectToServer()
        {
            IPEndPoint serverEndPoint = new IPEndPoint(serverIp, PORT);

            try
            {
                mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mainSocket.Connect(new IPEndPoint(serverIp, PORT));

                logger.Log("Connected to " + serverEndPoint.Address.ToString() + ":" + serverEndPoint.Port + ".");
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

        protected override void OnSocketDestroy(Socket socket)
        {
            logger.Log("Server disconnected");
            Disconnect();
        }

    }
}
