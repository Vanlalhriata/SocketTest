using System.Net.Sockets;

namespace SocketCommon
{
    /// <summary>
    /// State object for reading socket client data asynchronously
    /// </summary>
    public class SocketState
    {
        public const int BufferSize = 1024;
        public Socket CurrentSocket = null;
        public byte[] DataBuffer = new byte[BufferSize];
        public string tag = "";
    }
}
