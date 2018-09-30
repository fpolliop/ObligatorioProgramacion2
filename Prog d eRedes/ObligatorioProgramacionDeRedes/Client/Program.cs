using DataManager;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public class Program
    {

        private static string userNickname = "";
        public static void Main(string[] args)
        {
            ClientController clientController = new ClientController();
            //debeeria hablar con clientController. no con clientProtocol
            Console.WriteLine("Conectando al servidor...");
            Socket socket = clientController.Connect();

            Frame frameRequest = null;
            bool isLogged = false;

            while (!isLogged )
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

        private static void Menu(Socket socket, string user)
        {
            bool exit = false;
            while (!exit)
            {

                Console.WriteLine("*-*-*-*-*-*-*-*-*" + user + " Menu:*-*-*-*-*-*-*-*-*");
                Console.WriteLine("1- Listar Usuarios Conectados");
                Console.WriteLine("2- Listar Usuarios Registrados");
                Console.WriteLine("3- ");
                Console.WriteLine("4- Unirse a un Juego");
                Console.WriteLine("5- ");
                Console.WriteLine("6- ");
                Console.WriteLine("7- Salir");
                Console.WriteLine("Ingrese una opcion: ");
                int option = GetOption(1, 8);
                ActionType action = (ActionType)option;
                switch (action)
                {
                    case ActionType.ListConnectedUsers:
                        ClientController.ListConnectedUsers(socket);
                        ClearConsole();
                        break;
                    case ActionType.ListRegisteredUsers:
                        ClientController.ListRegisteredUsers(socket);
                        break;
                    case ActionType.JoinGame:
                        break;
                    case ActionType.JoinMatch:
                        Console.WriteLine("Seleccione su Rol:");
                        Console.WriteLine("1 - Monstruo");
                        Console.WriteLine("2 - Sobreviviente");
                        int role = GetOption(1, 2) - 1;
                        ClientController.JoinMatch(socket, userNickname, role);
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
