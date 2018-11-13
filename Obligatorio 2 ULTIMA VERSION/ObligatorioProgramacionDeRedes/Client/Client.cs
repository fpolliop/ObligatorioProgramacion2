using Protocol;
using System;
using System.Configuration;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static Protocol.Frame;

namespace Client
{
    public class Client
    {
        private ClientController ClientController { get; set; }
        private static int CLIENT_PORT = Int32.Parse(ConfigurationSettings.AppSettings["ClientPort"].ToString());
        private static int SERVER_PORT = Int32.Parse(ConfigurationSettings.AppSettings["ServerPort"].ToString());
        private static string CLIENT_IP = ConfigurationSettings.AppSettings["ClientIp"].ToString();
        private static string SERVER_IP = ConfigurationSettings.AppSettings["ServerIp"].ToString();
        private Socket socket;
        private Socket socketNotify;
        private string userNickname = "";
        private Image userAvatar;

        public Client()
        {
            ClientController = new ClientController();
        }

        public void StartClient()
        {
            Console.WriteLine("Conectando al servidor...");
            try
            {
                ConnectToServer();
                Thread notify = new Thread(() => HandleNotify(socketNotify));
                notify.Start();

                Frame frameRequest = null;
                bool isLogged = false;

                while (!isLogged)
                {
                    while (userNickname.Equals(""))
                    {
                        Console.WriteLine("Ingrese su nickname:");
                        userNickname = Console.ReadLine().Trim();
                        Console.Title = userNickname;
                        InsertAvatar();

                    }
                    string messageToSend = userNickname;

                    frameRequest = new Frame(Command.ConnectToServer, messageToSend);
                    FrameConnection.Send(socket, frameRequest);


                    FrameConnection.SendAvatar(socket, userAvatar);
                    Frame frameResponse = FrameConnection.Receive(socket);

                    string isConnected = frameResponse.Data;
                    if (isConnected.Equals("OK"))
                    {
                        isLogged = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Se ha conectado al servidor.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Presione enter para continuar.");
                        Console.ReadLine();
                        Console.Clear();
                    }
                    else if (isConnected.Equals("REPEATED"))
                    {
                        Console.WriteLine("Ya existe un jugador conectado con ese nickname.");
                        userNickname = "";
                    }
                    else if (isConnected.Equals("EXISTENT"))
                    {
                        isLogged = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Se ha conectado al servidor.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Presione enter para continuar.");
                        Console.ReadLine();
                        Console.Clear();
                    }
                    else
                    {
                        Console.WriteLine("No se ha podido establecer conexión");
                        isLogged = true;
                    }
                }
                if (isLogged)
                {
                    Menu(socket, userNickname);
                }
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Console.WriteLine("Gracias por jugar a SLASHER, presione enter para cerrar.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                
                Console.WriteLine("No se ha podido establecer la conexión con el Servidor");
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Gracias por jugar a SLASHER, presione una tecla para cerrar.");
                    Console.ReadLine();
                }
            }
        }

        private void InsertAvatar()
        {
            Console.WriteLine("Ingrese su Avatar:");
            bool avatarIsValid = false;
            while (!avatarIsValid)
            {
                try
                {
                    string avatarPath = Console.ReadLine();
                    userAvatar = new Bitmap(avatarPath);
                    avatarIsValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Avatar invalido. Ingrese la ruta de la imagen");
                }
            }
        }

        private void ConnectToServer()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Parse(CLIENT_IP), CLIENT_PORT);
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(SERVER_IP), SERVER_PORT);

            IPEndPoint localEpNotify = new IPEndPoint(IPAddress.Parse(CLIENT_IP), 0);
            socketNotify = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(clientEndpoint);
            socket.Connect(serverEndpoint);

            socketNotify.Bind(localEpNotify);
            socketNotify.Connect(serverEndpoint);
        }

        private void HandleNotify(Socket socket)
        {
            while (socket.Connected)
            {
                try
                {
                    Frame frameResponse = FrameConnection.Receive(socket);
                    if (frameResponse.Cmd == Command.Exit)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    else
                    {
                        string messageNotify = frameResponse.Data;
                        Console.WriteLine("NOTIFICACION: " + messageNotify);
                        ClientController.isInActiveMatch = false;
                    }
                }
                catch (Exception e)
                {
                    Environment.Exit(0);
                }
            }
        }

        private void Menu(Socket socket, string user)
        {
            bool exit = false;
            while (!exit)
            {

                Console.WriteLine("*-*-*-*-*-*-*-*-*" + user + " Menu Principal:*-*-*-*-*-*-*-*-*");
                Console.WriteLine("1- Listar Usuarios Conectados");
                Console.WriteLine("2- Listar Usuarios Registrados");
                Console.WriteLine("3- Unirse a una Partida");
                Console.WriteLine("4- Salir");
                Console.WriteLine("Ingrese una opcion: ");
                int option = GetOption(1, 4);
                Command cmd = (Command)option;
                switch (cmd)
                {
                    case Command.ListConnectedUsers:
                        ClientController.ListConnectedUsers(socket);
                        ClearConsole();
                        break;
                    case Command.ListRegisteredUsers:
                        ClientController.ListRegisteredUsers(socket);
                        ClearConsole();
                        break;
                    case Command.JoinMatch:
                        Console.WriteLine("Seleccione su Rol:");
                        Console.WriteLine("1 - Monstruo");
                        Console.WriteLine("2 - Sobreviviente");
                        int role = GetOption(1, 2) - 1;
                        ClientController.JoinMatch(socket, user, role);
                        while (ClientController.isInActiveMatch)
                        {
                            InGameMenu(socket, user);
                        }
                        ClearConsole();
                        break;
                    case Command.Exit:
                        ClientController.Exit(socket, user);
                        exit = true;
                        break;
                }
            }
        }

        private void ClearConsole()
        {
            Console.WriteLine("Presione enter para continuar.");
            Console.ReadLine();
            Console.Clear();
        }

        private int GetOption(int start, int end)
        {
            int option = -1;
            bool isCorrect = false;
            while (!isCorrect)
            {
                try
                {
                    Console.WriteLine("Ingrese la opcion correcta [" + start + " - " + end + "]: ");
                    option = Int32.Parse(Console.ReadLine());
                    if (option >= start && option <= end)
                    {
                        isCorrect = true;
                    }
                }
                catch (FormatException e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Su opcion no es correcta.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            return option;
        }

        private void InGameMenu(Socket socket, string user)
        {
            ClearConsole();
            Console.WriteLine("*-*-*-*-*-*-*-*-*" + user + " Menu Partida:*-*-*-*-*-*-*-*-*");
            Console.WriteLine("1- Mover");
            Console.WriteLine("2- Atacar");
            Console.WriteLine("Ingrese una opcion: ");
            int option = GetOption(1, 2) + 4;
            Command cmd = (Command)option;
            switch (cmd)
            {
                case Command.MovePlayer:
                    Console.WriteLine("Seleccione la direccion del movimiento:");
                    Console.WriteLine("1 - Arriba");
                    Console.WriteLine("2 - Derecha");
                    Console.WriteLine("3 - Abajo");
                    Console.WriteLine("4 - Izquierda");

                    int movementDirection = GetOption(1, 4) - 1;
                    if (!ClientController.isInActiveMatch) break;
                    ClientController.MovePlayer(socket, user, movementDirection);
                    break;
                case Command.AttackPlayer:
                    Console.WriteLine("Seleccione la direccion del ataque:");
                    Console.WriteLine("1 - Arriba");
                    Console.WriteLine("2 - Derecha");
                    Console.WriteLine("3 - Abajo");
                    Console.WriteLine("4 - Izquierda");
                    int attackDirection = GetOption(1, 4) + 3;
                    if (!ClientController.isInActiveMatch) break; 
                    ClientController.AttackPlayer(socket, user, attackDirection);
                    break;
            }
        }
    }
}
