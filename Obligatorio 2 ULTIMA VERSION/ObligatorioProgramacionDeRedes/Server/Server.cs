using Entities;
using Protocol;
using System;
using System.Configuration;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using static Protocol.Frame;
using System.Collections.Generic;

namespace Server
{
    public class Server : RemotingShared
    {
        private ServerController ServerController { get; set; }
        public static Repository usersList = new Repository();
        private Socket server;
        private static int SERVER_PORT = Int32.Parse(ConfigurationSettings.AppSettings["Port"].ToString());
        private static string SERVER_IP = ConfigurationSettings.AppSettings["Ip"].ToString();
        private static string ROUTE = ConfigurationSettings.AppSettings["Route"].ToString();
        private static string IMAGE_EXTENSION = ConfigurationSettings.AppSettings["ImageExtension"].ToString();
        bool serverIsActive;


        public Server()
        {
            ServerController = new ServerController(usersList);
        }

        public void StartServer()
        {
            Console.Title = "Server";
            //usersList = new Repository();
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress localAddress = IPAddress.Parse(SERVER_IP);
            IPEndPoint listener = new IPEndPoint(localAddress, SERVER_PORT);
            server.Bind(listener);
            server.Listen(100);
            serverIsActive = true;

            TcpChannel lTcpChannel = new TcpChannel(3333);
            ChannelServices.RegisterChannel(lTcpChannel, true);
            Type lRemotingSharedType = typeof(Server);
            RemotingConfiguration.ApplicationName = "MyRemotingNameApp";
            RemotingConfiguration.RegisterWellKnownServiceType(lRemotingSharedType, "MyRemotingName", WellKnownObjectMode.Singleton);
            try
            {
                while (serverIsActive)
                {
                    Thread shutdownServerOption = new Thread(() => ShutdownServerOption());
                    shutdownServerOption.Start();
                    Console.WriteLine("Esperando conexion con el cliente...");
                    Socket client = server.Accept();
                    Socket clientNotify = server.Accept();
                    Thread processClient = new Thread(() => ProcessClient(client, clientNotify));
                    processClient.Start();             
                }
            }
            catch
            {
                Console.Write("Ha ocurrido algo");
            }
        }

        private void ShutdownServerOption()
        {
            Console.WriteLine("Ingrese 'EXIT' para apagar el servidor");
            string exitOption = "";
            while (!exitOption.Equals("EXIT"))
            {
                exitOption = Console.ReadLine();
                if (exitOption.Equals("EXIT"))
                {
                    serverIsActive = false;
                    Environment.Exit(0);
                }
            }
        }

        private void ProcessClient(Socket client, Socket clientNotify)
        {
            Frame frameReceived = null;
            string userNickname = "";
            Image userAvatar = null;
            bool socketIsClosed = false;

            while (!socketIsClosed)
            {
                try
                {
                    frameReceived = FrameConnection.Receive(client);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    socketIsClosed = true;
                }

                try
                {
                    switch (frameReceived.Cmd)
                    {
                        case Command.ConnectToServer:

                            userAvatar = FrameConnection.ReceiveAvatar(client);
                            string response = ServerController.Connect(frameReceived, usersList.GetUsers());
                            if (response.Equals("OK"))
                            {
                                userNickname = frameReceived.GetUserNickname();
                                try
                                {
                                    userAvatar.Save(ROUTE + userNickname + IMAGE_EXTENSION);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Hubo un error al guardar la imagen en el servidor");
                                }
                                AddUserInList(userNickname, userAvatar, clientNotify);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Conectado el cliente: " + userNickname);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else if (response.Equals("EXISTENT"))
                            {
                                userNickname = frameReceived.GetUserNickname();
                                ConnectUser(userNickname, clientNotify);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Conectado el cliente: " + userNickname);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else if (response.Equals("REPEATED"))
                            {
                                userNickname = "";
                            }

                            Frame frame = new Frame(Command.ConnectToServer, response);
                            FrameConnection.Send(client, frame);
                            break;
                        case Command.ListConnectedUsers:
                            ServerController.ListConnectedUsers(client, usersList.GetUsers());
                            break;
                        case Command.ListRegisteredUsers:
                            ServerController.ListRegisteredUsers(client, usersList.GetUsers());
                            break;
                        case Command.JoinMatch:
                            userNickname = frameReceived.GetUserNickname();
                            User user = usersList.GetUserByNickname(userNickname);
                            Role role = frameReceived.GetUserRole();
                            ServerController.JoinPlayerToMatch(client, user, role);
                            break;
                        case Command.MovePlayer:
                            userNickname = frameReceived.GetUserNickname();
                            User player = usersList.GetUserByNickname(userNickname);
                            PlayerGameAction gameAction = frameReceived.GetPlayerGameAction();
                            ServerController.MovePlayer(client, player, gameAction, usersList);
                            break;
                        case Command.AttackPlayer:
                            userNickname = frameReceived.GetUserNickname();
                            User attacker = usersList.GetUserByNickname(userNickname);
                            PlayerGameAction action = frameReceived.GetPlayerGameAction();
                            ServerController.AttackPlayer(client, attacker, action, usersList);
                            break;
                        case Command.Exit:
                            userNickname = frameReceived.GetUserNickname();
                            ServerController.Exit(client, userNickname, usersList);
                            DisconnectUser(userNickname);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Desconectado el cliente: " + userNickname);
                            Console.ForegroundColor = ConsoleColor.White;
                            socketIsClosed = true;
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Ha sucedido un error inesperado");

                    if (!userNickname.Equals(""))
                    {
                        DisconnectUser(userNickname);
                        ServerController.RemoveFromActiveMatch(userNickname);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Desconectado el cliente: " + userNickname);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    socketIsClosed = true;
                }
            }
            client.Close();
        }

        private void ConnectUser(string nickname, Socket clientNotify)
        {
            User user = usersList.GetUserByNickname(nickname);
            if (user != null)
            {
                user.IsConnected = true;
                user.SocketNotify = clientNotify;
            }
        }

        private void AddUserInList(string nickname, Image userAvatar, Socket socketNotify)
        {
            User user = new User(nickname, userAvatar, socketNotify);
            usersList.AddUser(user);
        }

        private void DisconnectUser(string nickname)
        {
            User user = usersList.GetUserByNickname(nickname);
            if (user != null)
            {
                user.IsConnected = false;
            }
            else
            {
                Console.WriteLine("Error! No se ha podido desconectar el usuario: " + nickname);
            }
        }

        public override bool AddUser(string name)
        {
            User userExists = usersList.GetUserByNickname(name);
            if (userExists == null)
            {
                User newUser = new User(name);
                //lock (userLocker)
                // {
                usersList.AddUser(newUser);

                //}
                return true;
            }
            return false;
        }

        public override bool DeleteUser(string name)
        {
            User foundUser = usersList.GetUserByNickname(name);
            if (foundUser != null && !foundUser.IsConnected)
            {
                usersList.RemoveUser(foundUser);
                return true;
            }
            return false;
        }

        public override List<User> GetUsers()
        {
            return usersList.GetUsers();
        }

        public override bool ModifyUser(string name, string newName)
        {
            return usersList.ModifyUser(name, newName);
        }
        public override List<Ranking> GetRankings()
        {
            return usersList.GetRankings();
        }
    }
}
