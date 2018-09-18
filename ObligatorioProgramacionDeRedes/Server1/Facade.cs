using ObligatorioProgramacionDeRedes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server1
{
    class Facade
    {
        private static Server gameServer;
        
        public static void Run()
        {
            
            gameServer = new Server();
            

        }

        internal static void StartGame()
        {
            var serverThread = new Thread(() => gameServer.Start());
            serverThread.Start();
        }
    }
}
