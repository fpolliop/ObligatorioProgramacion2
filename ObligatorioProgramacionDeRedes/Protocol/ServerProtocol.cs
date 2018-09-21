using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DataManager;
using Controllers;

namespace Protocol
{
    public class ServerProtocol
    {
        private bool serverIsActive = true;
        private Socket server;
        private ServerController serverController = new ServerController();
        int clientPort = 1099;
        int serverPort = 1098;
        string IP = "127.0.0.1";
        //string IP = "192.168.1.54";
        private byte[] msgLengthInBytes;

        private int DATA_LENGTH = 4;


        public void StartServer()
        {
            server = new Socket(SocketType.Stream, ProtocolType.Tcp);

            IPAddress localAdd = IPAddress.Parse(IP);
            IPEndPoint listener = new IPEndPoint(localAdd, serverPort);
            server.Bind(listener);
            server.Listen(100);

            var thread = new Thread(() => Listen()); //Este thread esta bien?? preguntar
            thread.Start();
        }

        private void Listen()
        {
            while (serverIsActive)
            {
                var client = server.Accept();
                var thread = new Thread(() => ProcessClient(client));
                thread.Start();

            }
        }

        private void ProcessClient(Socket client)
        {
            Connection serverConnection = new Connection(client);
            string messageReceived = serverConnection.ReceiveMessage();
            string[] clientRequest = MessageDecoder.DecodeMessage(messageReceived);
            serverController.HandleClientRequest(clientRequest);

            client.Shutdown(SocketShutdown.Both);
            client.Close();
           
        }
    }
}