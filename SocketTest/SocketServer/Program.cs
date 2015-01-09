using SocketCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            server = new SocketServerManager(logger);
            server.StartServer();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            server.StopServer();
        }
    }
}
