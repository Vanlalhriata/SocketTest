using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconServer
{
    class Program
    {
        static Program program;

        static void Main(string[] args)
        {
            program = new Program();
        }

        private BeaconServer beaconServer;
        
        public Program()
        {
            beaconServer = new BeaconServer();
            beaconServer.Initialise(onEnter, onExit);
        }

        private void onEnter(int minorCode)
        {
            Console.WriteLine(minorCode + " in");
        }

        private void onExit(int minorCode)
        {
            Console.WriteLine(minorCode + " out");
        }
    }
}
