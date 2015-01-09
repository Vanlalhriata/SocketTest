using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SocketCommon
{
    public class SocketServerManager : SocketManagerBase
    {

        private List<Socket> workerSockets; // These sockets will handle clients

        public SocketServerManager(ILogger pLogger, Action<byte[]> pReceiveCallback)
            : base(pLogger, pReceiveCallback)
        {
            workerSockets = new List<Socket>();
        }

        public void StartServer()
        {
            // Get the local ip
            IPHostEntry hostEntry = Dns.GetHostEntry("");
            IPAddress localIp = hostEntry.AddressList[1];
            IPEndPoint localEndpoint = new IPEndPoint(localIp, PORT);

            try
            {
                // Initialize the main socket
                logger.Log("Starting server.");
                mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mainSocket.Bind(localEndpoint);

                // Start listening for new connection requests
                mainSocket.Listen(4);
                mainSocket.BeginAccept(new AsyncCallback(OnClientConnect), mainSocket);
                logger.Log("Listening at " + localEndpoint.Address.ToString() + ":" + localEndpoint.Port + "...");
            }
            catch (SocketException ex)
            {
                logger.Log(ex.Message);
                throw ex;
            }
        }

        public void StopServer()
        {
            // Close all sockets connected to clients
            foreach (Socket socket in workerSockets)
                CloseSocket(socket);

            DisconnectMainSocket();

            logger.Log("Stopped server");
        }

        public void OnClientConnect(IAsyncResult asyncResult)
        {
            try
            {
                // Accept new connection request -> new worker socket
                Socket resultSocket = (Socket)asyncResult.AsyncState;
                Socket workerSocket = resultSocket.EndAccept(asyncResult);

                logger.Log("Client connected.");
                workerSockets.Add(workerSocket);

                WaitForData(workerSocket);

                // Wait for the next connection request
                mainSocket.BeginAccept(new AsyncCallback(OnClientConnect), mainSocket);
            }
            catch (SocketException ex)
            {
                logger.Log(ex.Message);
                throw ex;
            }
            catch (ObjectDisposedException)
            {
                // When mainSocket closes, this callback triggers and this exception is thrown
                logger.Log("Main server socket closed");
            }
        }

        public override void Send(byte[] bytesToSend)
        {
            try
            {
                foreach (Socket socket in workerSockets)
                    socket.Send(bytesToSend);
            }
            catch (SocketException ex)
            {
                logger.Log(ex.Message);
                throw ex;
            }
        }

        protected override void OnSocketDestroy(Socket socket)
        {
            logger.Log("Client exited. Closing connection");
        }

    }
}
