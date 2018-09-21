using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            ClientController clientController = clientController = new ClientController();
            //debeeria hablar con clientController. no con clientProtocol
            Console.WriteLine("Conectando al servidor...");
            if (clientController.Connect())
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
            


        }
    }
}
