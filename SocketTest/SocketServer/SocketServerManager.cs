using SocketCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    public class SocketServerManager
    {

        private const int PORT = 8095;

        private ILogger logger;
        private Socket mainSocket;  // The socket which will listen for and assign connections
        private List<Socket> workerSockets; // These sockets will handle clients

        public SocketServerManager(ILogger pLogger)
        {
            logger = pLogger;
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
            if (null != mainSocket)
                CloseSocket(mainSocket);

            // TODO: Close sockets assigned to clients

            logger.Log("Stopped server");
        }

        private void WaitForData(Socket socket)
        {
            // Prepare the socket state
            SocketState state = new SocketState();
            state.CurrentSocket = socket;
            state.tag = "Server worker";

            socket.BeginReceive(state.DataBuffer,
                                0, state.DataBuffer.Length,
                                SocketFlags.None,
                                new AsyncCallback(OnDataReceived),
                                state);
        }

        private void CloseSocket(Socket socket)
        {
            if (socket.Connected)
                socket.Shutdown(SocketShutdown.Both);

            socket.Close();
            socket = null;
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

        public void OnDataReceived(IAsyncResult asyncResult)
        {
            SocketState state = (SocketState)asyncResult.AsyncState;
            Socket workerSocket = state.CurrentSocket;

            try
            {
                // End the data receipt
                int numBytesReceived = workerSocket.EndReceive(asyncResult);

                // 0 bytes received -> socket closed
                if (numBytesReceived > 0)
                {
                    // TODO: process received data

                    // Wait for next data
                    WaitForData(workerSocket);
                }
                else
                {
                    // Handle socket closed
                    logger.Log("Client exited. Closing connection");
                    workerSockets.Remove(workerSocket);
                    CloseSocket(workerSocket);
                }

            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10054)
                {
                    // The connection was forcibly closed by the client
                    logger.Log("Client exited. Closing connection");
                    workerSockets.Remove(workerSocket);
                    CloseSocket(workerSocket);
                }
                else
                {
                    logger.Log(ex.Message);
                    throw ex;
                }
            }
        }

    }
}
