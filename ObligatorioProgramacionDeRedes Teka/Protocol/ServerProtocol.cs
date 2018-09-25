﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DataManager;
using System.Configuration;
using System.Text;

namespace Protocol
{
    public class ServerProtocol
    {

        private Socket server;
        private int serverPort = Int32.Parse(ConfigurationSettings.AppSettings["Port"].ToString());
        private string serverIp = ConfigurationSettings.AppSettings["Ip"].ToString();

        public Socket StartServer()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress localAddress = IPAddress.Parse(serverIp);
            IPEndPoint listener = new IPEndPoint(localAddress, serverPort);
            server.Bind(listener);
            server.Listen(100);
            return server;
        }
    }
}