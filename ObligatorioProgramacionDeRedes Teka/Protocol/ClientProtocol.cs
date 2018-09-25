using DataManager;
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
        private Connection connection;

        public void Connect()
        {
            try
            {
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Parse(clientIp), 0);
                IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
                client.Bind(clientEndpoint);
                client.Connect(serverEndpoint);
                connection = new Connection(client);
                Frame connectionRequestFrame = new Frame(ActionType.ConnectToServer, "");
                
                connection.SendMessage(connectionRequestFrame);
            }
            catch
            {
                //??
            }
        }

        public void Close()
        {
            connection.Close();
        }

        public void Send(Frame frameToSend)
        {
            connection.SendMessage(frameToSend);
        }

        public Frame Receive()
        {
            Frame receivedFrame = connection.ReceiveMessage();
            return receivedFrame;

        }
    }
}
