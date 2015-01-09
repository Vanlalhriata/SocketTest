using SocketCommon;
using System;
using System.Text;

namespace SocketServer
{
    class Program
    {
        static Program program;

        static void Main(string[] args)
        {
            program = new Program();
        }

        private SocketServerManager server;
        private Logger logger;

        public Program()
        {
            logger = new Logger();
            server = new SocketServerManager(logger, OnReceiveData);
            server.StartServer();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            server.StopServer();
        }

        private void OnReceiveData(byte[] receivedData)
        {
            string msg = Encoding.ASCII.GetString(receivedData);
            Console.WriteLine("Received: " + msg);

            server.Send(receivedData);
        }
    }
}
