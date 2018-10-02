using DataManager;
using Protocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public class Program
    {
        private static int clientPort = Int32.Parse(ConfigurationSettings.AppSettings["ClientPort"].ToString());
        private static int serverPort = Int32.Parse(ConfigurationSettings.AppSettings["ServerPort"].ToString());
        private static string clientIp = ConfigurationSettings.AppSettings["ClientIp"].ToString();
        private static string serverIp = ConfigurationSettings.AppSettings["ServerIp"].ToString();
        private static string userNickname = "";
        public static void Main(string[] args)
        {
           
        Console.WriteLine("Conectando al servidor...");
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Parse(clientIp), 0);
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            IPEndPoint localEpNotify = new IPEndPoint(IPAddress.Parse(clientIp), 0);
            Socket socketNotify = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(clientEndpoint);
            socket.Connect(serverEndpoint);

            socketNotify.Bind(localEpNotify);
            socketNotify.Connect(serverEndpoint);

            Thread notifies = new Thread(() => HandleNotify(socketNotify));

            notifies.Start();

            Frame frameRequest = null;
            bool isLogged = false;

            while (!isLogged)
            {
                while (userNickname.Equals(""))
                {
                    Console.WriteLine("Ingrese su nickname:");
                    userNickname = Console.ReadLine().Trim();
                    Console.Title = userNickname;
                }
                string messageToSend = userNickname;

                frameRequest = new Frame(ActionType.ConnectToServer, messageToSend);
                FrameConnection.Send(socket, frameRequest);
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
                else if (isConnected.Equals("REPEAT"))
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

        private static void HandleNotify(Socket socket)
        {
            while (socket.Connected)
            {
                try
                {
                    Frame frameResponse = FrameConnection.Receive(socket);
                    if (frameResponse.Action == ActionType.Exit)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    else
                    {
                        string messageNotify = frameResponse.Data;
                        Console.WriteLine("NOTIFICACION: " + messageNotify);
                        ClientController.isInActiveMatch = false;
                        //Console.ReadLine();
                    }
                }
                catch (Exception e)
                {
                    Environment.Exit(0);
                }
            }
        }

        private static void Menu(Socket socket, string user)
        {
            bool exit = false;
            while (!exit)
            {

                Console.WriteLine("*-*-*-*-*-*-*-*-*" + user + " Menu:*-*-*-*-*-*-*-*-*");
                Console.WriteLine("1- Listar Usuarios Conectados");
                Console.WriteLine("2- Listar Usuarios Registrados");
                Console.WriteLine("3- Unirse a una Partida");
                Console.WriteLine("4- Salir");
                Console.WriteLine("Ingrese una opcion: ");
                int option = GetOption(1, 4);
                ActionType action = (ActionType)option;
                switch (action)
                {
                    case ActionType.ListConnectedUsers:
                        ClientController.ListConnectedUsers(socket);
                        ClearConsole();
                        break;
                    case ActionType.ListRegisteredUsers:
                        ClientController.ListRegisteredUsers(socket);
                        ClearConsole();
                        break;
                    case ActionType.JoinMatch:
                        Console.WriteLine("Seleccione su Rol:");
                        Console.WriteLine("1 - Monstruo");
                        Console.WriteLine("2 - Sobreviviente");
                        int role = GetOption(1, 2) - 1;
                        ClientController.JoinMatch(socket, user, role);
                        while (ClientController.isInActiveMatch)
                        {
                            inGameMenu( socket,  user);
                        }
                        ClearConsole();
                        break;
                    case ActionType.SelectRole:
                        break;
                    case ActionType.MovePlayer:
                        break;
                    case ActionType.AttackPlayer:
                        break;
                    case ActionType.Exit:
                        ClientController.Exit(socket, user);
                        exit = true;
                        break;
                }
            }
        }

        private static void ClearConsole()
        {
            Console.WriteLine("Presione enter para continuar.");
            Console.ReadLine();
            Console.Clear();
        }

        private static int GetOption(int start, int end)
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

        private static void inGameMenu(Socket socket, string user)
        {
            ClearConsole();
            Console.WriteLine("*-*-*-*-*-*-*-*-*" + user + " Menu Partida:*-*-*-*-*-*-*-*-*");
            Console.WriteLine("1- Mover");
            Console.WriteLine("2- Atacar");
            Console.WriteLine("Ingrese una opcion: ");
            int option = GetOption(1, 2) + 5;
            ActionType action = (ActionType)option;
            switch (action)
            {
                case ActionType.MovePlayer:
                    Console.WriteLine("Seleccione la direccion del movimiento:");
                    Console.WriteLine("1 - Arriba");
                    Console.WriteLine("2 - Derecha");
                    Console.WriteLine("3 - Abajo");
                    Console.WriteLine("4 - Izquierda");

                    int movementDirection = GetOption(1, 4) - 1;
                    if (!ClientController.isInActiveMatch)
                    {
                        break;
                    }
                    ClientController.MovePlayer(socket, user, movementDirection);
                    break;
                case ActionType.AttackPlayer:
                    Console.WriteLine("Seleccione la direccion del ataque:");
                    Console.WriteLine("1 - Arriba");
                    Console.WriteLine("2 - Derecha");
                    Console.WriteLine("3 - Abajo");
                    Console.WriteLine("4 - Izquierda");
                    int attackDirection = GetOption(1, 4) + 3;
                    if (!ClientController.isInActiveMatch)
                    {
                        break;
                    }
                    ClientController.AttackPlayer(socket, user, attackDirection);
                    break;
            }
        }
        /*if (clientController.Connect())
        {
            Console.WriteLine("Conectado al servidor");
            Console.WriteLine("Bienvenido al Juego");

            Console.WriteLine("Elija una opcion:");
            Console.WriteLine("1 - Crear Jugador");
            Console.WriteLine("2 - Unirse al Juego");
            Console.WriteLine("3 - Seleccionar Rol");
            Console.WriteLine("4 - Mover Jugador");
            Console.WriteLine("5 - Atacar Jugador");
            string optionString = Console.ReadLine();
            try
            {
                int option = int.Parse(optionString);
                if (option == 1)
                {
                    //agregar jugador
                    Console.WriteLine("Ingrese Su Nickname");
                    string nickname = Console.ReadLine();
                    clientController.CreatePlayer(nickname);
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine("Los datos que ingreso no son validos.");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
            }



        }
        else
        {
            Console.WriteLine("Error. No se pudo conectar al servidor. Intente de nuevo");
            Console.ReadLine();
        }



    }*/
    }
}
