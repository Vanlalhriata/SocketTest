using SocketCommon;
using System;
using System.Net;
using System.Text;

namespace SocketClient
{
    class Program
    {
        static Program program;

        static void Main(string[] args)
        {
            program = new Program();
        }

        private SocketClientManager client;
        private Logger logger;

        public Program()
        {
            IPAddress serverIp = IPAddress.Parse("192.168.2.2");

            logger = new Logger();
            client = new SocketClientManager(logger, OnReceiveData, serverIp);
            client.ConnectToServer();

            string msg = "";

            Console.Write("Message: ");
            msg = Console.ReadLine();

            while (!msg.Equals("exit", StringComparison.OrdinalIgnoreCase) && client.IsConnected)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(msg);
                client.Send(bytes);

                Console.Write("Message: ");
                msg = Console.ReadLine();
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            client.Disconnect();
        }

        private void OnReceiveData(byte[] receivedData)
        {
            string msg = Encoding.ASCII.GetString(receivedData);
            Console.WriteLine("Received: " + msg);
        }
    }
}
