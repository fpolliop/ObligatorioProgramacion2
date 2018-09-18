using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ObligatorioProgramacionDeRedes
{
    public class Server
    {
        int port = 1099;
        string IP = "127.0.0.1"; 
        //string IP = "192.168.1.54";
        private byte[] msgLengthInBytes;
        private bool serverIsActive = true;

        private int DATA_LENGTH = 4;

        public void Start()
        {
            var server = new Socket(SocketType.Stream, ProtocolType.Tcp);

            IPAddress localAdd = IPAddress.Parse(IP);
            IPEndPoint listener = new IPEndPoint(localAdd, port);
            server.Bind(listener);
            server.Listen(100);
            
            while(serverIsActive)
            {
                //new thread?
                var client = server.Accept();
                var received = 0;

                var thread = new Thread(() => ProcessClient(client, received));
                thread.Start();
                
            }
        }

        private void ProcessClient(Socket client, int received)
        {
            var dataInBytes = new byte[DATA_LENGTH];

            while (received < DATA_LENGTH)
            {
                var pos = client.Receive(dataInBytes, received, 4 - received, SocketFlags.None);
                if (pos == 0) throw new SocketException();

                received += pos;
            }

            var dataLength = BitConverter.ToInt32(dataInBytes, 0);
            var dataReceived = new byte[dataLength];
            received = 0;
            while (received < dataLength)
            {
                var pos = client.Receive(dataReceived, received, dataLength - received, SocketFlags.None);
                if (pos == 0) throw new SocketException();

                received += pos;
            }
            var message = Encoding.ASCII.GetString(dataReceived);
        }

        public void Stop()
        {
            serverIsActive = false;

        }
    }
}