using System;
using System.Net.Sockets;

namespace SocketCommon
{
    /// <summary>
    /// A base class for socket communication. For sending and receiving,
    /// subclasses should implement Send() and call WaitForData
    /// </summary>
    public abstract class SocketManagerBase
    {
        protected const int PORT = 8095;

        protected ILogger logger;
        protected Socket mainSocket;
        protected Action<byte[]> receiveCallback;   // Callback on receive data

        public SocketManagerBase(ILogger pLogger, Action<byte[]> pReceiveCallback)
        {
            logger = pLogger;
            receiveCallback = pReceiveCallback;
        }

        public abstract void Send(byte[] bytesToSend);

        protected virtual void OnSocketDestroy(Socket socket)
        {
            // Extra steps to be taken whenever a socket is destroyed
            // either because it asked (sent 0 bytes) or threw a 10054 error code
        }

        protected void WaitForData(Socket socket)
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
                    ProcessReceivedData(state.DataBuffer, numBytesReceived);

                    // Wait for next data
                    WaitForData(workerSocket);
                }
                else
                {
                    // Handle socket closed
                    OnSocketDestroy(workerSocket);
                    CloseSocket(workerSocket);
                }

            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10054)
                {
                    // The connection was forcibly closed by the client
                    OnSocketDestroy(workerSocket);
                    CloseSocket(workerSocket);
                }
                else
                {
                    logger.Log(ex.Message);
                    throw ex;
                }
            }
        }

        protected void ProcessReceivedData(byte[] receivedData, int size)
        {
            Array.Resize<byte>(ref receivedData, size);
            receiveCallback(receivedData);
        }

        protected void DisconnectMainSocket()
        {
            if (null != mainSocket)
                CloseSocket(mainSocket);
        }

        protected void CloseSocket(Socket socket)
        {
            if (socket.Connected)
                socket.Shutdown(SocketShutdown.Both);

            socket.Close();
            socket = null;
        }

    }
}
