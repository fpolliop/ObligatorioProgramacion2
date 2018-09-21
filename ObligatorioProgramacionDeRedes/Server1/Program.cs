using ObligatorioProgramacionDeRedes;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public class Program
    {
        private static ServerProtocol serverProtocol;
        static void Main(string[] args)
        {
            serverProtocol = new ServerProtocol();
            serverProtocol.StartServer();
            Console.ReadLine();
        }
    }
}
