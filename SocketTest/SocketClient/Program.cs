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

            string msg = "";

            Console.Write("Message: ");
            msg = Console.ReadLine();

            while (!msg.Equals("exit", StringComparison.OrdinalIgnoreCase))
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
    }
}
