using SocketCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconServer
{
    public class BeaconServer
    {

        private const string ENTER_CODE = "EN";
        private const string EXIT_CODE = "EX";

        private Logger logger;
        private SocketServerManager server;

        private Action<int> onEnter;
        private Action<int> onExit;

        public void Initialise(Action<int> enterCallback, Action<int> exitCallback)
        {
            onEnter = enterCallback;
            onExit = exitCallback;

            logger = new Logger();
            server = new SocketServerManager(logger, onReceive);

            server.StartServer();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            server.StopServer();
        }

        private void onReceive(byte[] receivedBytes)
        {
            string receivedString = Encoding.ASCII.GetString(receivedBytes);

            string minorString = receivedString.Substring(2);
            int minorInt = 0;

            if (!int.TryParse(minorString, out minorInt))
            {
                // Invalid code received
                logInvalidMessage(receivedString);
                return;
            }

            string code = receivedString.Substring(0, 2);

            if (code.Equals(ENTER_CODE, StringComparison.OrdinalIgnoreCase))
            {
                if (null != onEnter)
                    onEnter(minorInt);
            }
            else if (code.Equals(EXIT_CODE, StringComparison.OrdinalIgnoreCase))
            {
                if (null != onExit)
                    onExit(minorInt);
            }
            else
            {
                // Invalid code received
                logInvalidMessage(receivedString);
                return;
            }
        }

        private void logInvalidMessage(string message)
        {
            logger.Log("BeaconServer: Invalid messaged received from client: " + message);
        }

    }
}
