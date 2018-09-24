using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class ClientProtocol
    {
        private int clientPort = Int32.Parse(ConfigurationSettings.AppSettings["ClientPort"].ToString());
        private int serverPort = Int32.Parse(ConfigurationSettings.AppSettings["ServerPort"].ToString());
        private string clientIp = ConfigurationSettings.AppSettings["ClientIp"].ToString();
        private string serverIp = ConfigurationSettings.AppSettings["ServerIp"].ToString();
        
        public Socket Connect()
        {
            try
            {
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Parse(clientIp), 0);
                IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
                client.Bind(clientEndpoint);
                client.Connect(serverEndpoint);
                return client;
                /*Connection connection = new Connection(client);

                //hacerlo mejor onda automatico
                connection.SendMessage("REQ|00|1123|conectarse al servidor");
                connection.Close();
                client.Close();*/
            }
            catch
            {
                return null; // CAMBIAR ESTO
            }
        }

        public void SendRequestMessage(string encodedMessage)
        {
            var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           // client.Bind(clientEndpoint);
           // client.Connect(serverEndpoint);
            Connection connection = new Connection(client);

            connection.SendMessage(encodedMessage);
            connection.Close();
            client.Close();
        }
    }
}
