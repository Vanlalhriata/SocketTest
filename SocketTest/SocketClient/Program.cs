using SocketCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            logger = new Logger();
            client = new SocketClientManager(logger);
            client.ConnectToServer();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            client.Disconnect();
        }
    }
}
