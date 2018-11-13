﻿using AdminClient.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient
{
    class Program
    {
        public static bool Active { get; set; }

        static void Main(string[] args)
        {
            ServiceClient client = new ServiceClient();
            Active = true;

            while (Active)
            {
                int option = MainMenu();
                ProcessOption(option, client);
            }

        }

        private static int MainMenu()
        {
            Console.WriteLine("");
            Console.WriteLine("Menu Principal:");
            Console.WriteLine("1. Alta de usuario");
            Console.WriteLine("2. Baja de usuario");
            Console.WriteLine("3. Modificar usuario");
            Console.WriteLine("4. Rankings");
            Console.WriteLine("5. Estadisticas");
            Console.WriteLine("6. Salir");
            Console.WriteLine("Seleccione la opcion que desea realizar");

            string line = Console.ReadLine();
            int option = ConvertToInt(line);
            while (!(option <= 6 && option > 0) || !IsValidOption(line))
            {
                Console.WriteLine("Opcion no valida, seleccione una opcion correcta");
                line = Console.ReadLine();
                option = ConvertToInt(line);
            }
            return option;
        }

        public static bool IsValidOption(string word)
        {
            if (!(word.Length == 1))
            {
                return false;
            }
            string options = "123456";
            if (options.Contains(word))
            {
                return true;
            }
            return false;
        }

        public static int ConvertToInt(string line)
        {
            if (IsValidOption(line))
            {
                return Convert.ToInt32(line);
            }
            else
            {
                return 1000;
            }
        }

        private static void ProcessOption(int option, ServiceClient client)
        {
            switch (option)
            {
                case 1:
                    RegisterUser(client);
                    break;
                case 2:
                    DeleteUser(client);
                    break;
                case 3:
                    ModifyUser(client);
                    break;
                case 4:
                    Ranking(client);
                    break;
                case 5:
                    Statistics(client);
                    break;
                case 6:
                    CloseApp();
                    break;
            }
        }

        private static void RegisterUser(ServiceClient client)
        {
            string name = "";
            bool datoValidoUser = false;
            while (!datoValidoUser)
            {
                Console.WriteLine("Ingrese el nombre de usuario a registrar: ");
                name = Console.ReadLine();
                if (name.Length > 0)
                {
                    datoValidoUser = true;
                }
            }

            bool serverResponse = client.AddUser(name);

            if (serverResponse.Equals(true))
            {
                Console.WriteLine("Usuario registrado con exito.");


            }
            else
            {
                Console.WriteLine("No se pudo registrar, el usuario ya exite o algun dato esta vacio.");
            }
        }

        private static void DeleteUser(ServiceClient client)
        {
            /*Console.WriteLine("Lista de usuarios: ");

            List<User> users = client.GetUsers().ToList();

            int cont = 1;
            foreach (var user in users)
            {
                Console.WriteLine("{0}. " + user.Nickname, cont);
                cont++;
            }
            */
            Console.WriteLine("Escriba el nombre del usuario que desea eliminar: ");
            string toDelete = Console.ReadLine();

            bool serverResponse = client.DeleteUser(toDelete);

            if (serverResponse)
            {
                Console.WriteLine("Usuario eliminado con exito.");

            }
            else
            {
                Console.WriteLine("No se pudo eliminar, el usuario no existe o esta conectado.");
            }
        }

        private static void ModifyUser(ServiceClient client)
        {
            /*
            Console.WriteLine("Lista de usuarios: ");

            List<User> users = client.GetUsers().ToList();

            int cont = 1;
            foreach (var user in users)
            {
                Console.WriteLine("{0}. " + user.Nickname, cont);
                cont++;
            }
            */
            Console.WriteLine("Escriba el nombre del usuario que desea modificar: ");
            string nameToModify = Console.ReadLine();

            Console.WriteLine("Escriba el nuevo nombre de usuario: ");
            string newName = Console.ReadLine();

            bool serverResponse = client.ModifyUser(nameToModify, newName);

            if (serverResponse)
            {
                Console.WriteLine("Usuario modificado con exito.");

            }
            else
            {
                Console.WriteLine("No se pudo modificar, el usuario no exite o esta conectado.");
            }
        }

        private static void Ranking(ServiceClient client)
        {
            List<Ranking> rankings = client.GetRanking().ToList();
            foreach (Ranking rank in rankings)
            {
                Console.WriteLine($"Jugador: {rank.Nickname} como {rank.Role.ToString()} - fecha: {rank.Date}.  - Puntaje: {rank.Points}");
            }
        }

        private static void Statistics(ServiceClient client)
        {
            List<Statistic> statistics = client.GetStatistics().ToList();
            foreach (Statistic stat in statistics)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("- Partida del: " + stat.Date);
                foreach (GameStatistic gameStatistic in stat.GameStatistics)
                {
                    builder.Append("\n Jugador: " + gameStatistic.Nickname + "\n  -Rol: " + gameStatistic.Role + "\n  -Ha Ganado: " + gameStatistic.Result + ". \n");
                }
                Console.WriteLine(builder.ToString());
            }
        }

        private static void CloseApp()
        {
            Environment.Exit(0);
        }

    }
}
