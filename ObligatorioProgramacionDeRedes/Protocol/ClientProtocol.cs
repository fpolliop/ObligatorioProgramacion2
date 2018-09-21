using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class ClientProtocol
    {
        int clientPort = 1099;
        int serverPort = 1098;
        string IP = "127.0.0.1";
        //string IP = "192.168.1.54";
        private byte[] msgLengthInBytes;
        private bool serverIsActive = true;

        private int DATA_LENGTH = 4;

        private IPEndPoint clientEndpoint;
        private IPEndPoint serverEndpoint;

        public bool Connect()
        {
            try
            {
                var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientEndpoint = new IPEndPoint(IPAddress.Parse(IP), clientPort);
                serverEndpoint = new IPEndPoint(IPAddress.Parse(IP), serverPort);
                client.Bind(clientEndpoint);
                client.Connect(serverEndpoint);
                Connection connection = new Connection(client);

                //hacerlo mejor onda automatico
                connection.SendMessage("REQ|00|1123|conectarse al servidor");
                connection.Close();
                client.Close();
            } catch
            {
                return false;
            }
            return true;

        }

        public void SendRequestMessage(string encodedMessage)
        {
            var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Bind(clientEndpoint);
            client.Connect(serverEndpoint);
            Connection connection = new Connection(client);

            connection.SendMessage(encodedMessage);
            connection.Close();
            client.Close();
        }
    }
}
