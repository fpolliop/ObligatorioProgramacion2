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

        public static void Main(string[] args)
        {
            ClientController clientController = new ClientController();
            //debeeria hablar con clientController. no con clientProtocol
            Console.WriteLine("Conectando al servidor...");
            Socket socket = clientController.Connect();

            Frame frameRequest = null;
            string user = "";
            bool isLogged = false;
            bool isFinished = false;

            while (!isLogged && !isFinished)
            {
                while (user.Equals(""))
                {
                    Console.WriteLine("Ingrese su nickname:");
                    user = Console.ReadLine().Trim();
                    Console.Title = user;
                }
                string messageToSend = user;
                byte[] userData = Encoding.ASCII.GetBytes(messageToSend);
                frameRequest = new Frame(ActionType.ConnectToServer, userData);
                FrameConnection.Send(socket, frameRequest);
                Frame frameResponse = FrameConnection.Receive(socket);

                string isConnected = Encoding.ASCII.GetString(frameResponse.Data, 0, frameResponse.Data.Length);
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
                else if (isConnected.Equals("EXCEEDED"))
                {
                    Console.WriteLine("Suficientes usuarios conectados, intente más tarde.");
                    Console.ReadLine();
                    isFinished = true;
                }
                else if (isConnected.Equals("REPEAT"))
                {
                    Console.WriteLine("Ya existe un jugador conectado con ese nickname.");
                    user = "";
                }
                else
                {
                    Console.WriteLine("No se ha podido establecer conexión");
                    isFinished = true;
                }
            }
            if (isLogged)
            {
                Menu(socket, user);
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
                Console.WriteLine("4- ");
                Console.WriteLine("5- ");
                Console.WriteLine("6- ");
                Console.WriteLine("7- Salir");
                Console.WriteLine("Ingrese una opcion: ");
                int option = GetOption(1, 7);
                ActionType action = (ActionType)option;
                switch (action)
                {
                    case ActionType.ListConnectedUsers:
                        ClientController.ListConnectedUsers(socket);
                        Console.WriteLine("Presione enter para continuar.");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case ActionType.Exit:
                        ClientController.Exit(socket, user);
                        exit = true;
                        break;
                }
            }
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
