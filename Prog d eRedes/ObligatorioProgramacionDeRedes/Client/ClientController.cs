using DataManager;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientController
    {
        ClientProtocol clientProtocol;
        public static bool isInActiveMatch = false;

        public Socket Connect()
        {
            clientProtocol = new ClientProtocol();
            return clientProtocol.Connect();
        }

        public static void ListConnectedUsers(Socket socket)
        {
            Frame frameRequest = new Frame(ActionType.ListConnectedUsers, "");
            FrameConnection.Send(socket, frameRequest);
            Frame frameResponse = FrameConnection.Receive(socket);
            string[] listUsers = GetListFormatted(frameResponse.Data);
            if (!IsEmpty(listUsers))
            {
                System.Console.WriteLine("Usuarios Conectados: ");
                foreach (string user in listUsers)
                {
                    System.Console.WriteLine(user);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("No hay usuarios conectados");
                Console.ForegroundColor = ConsoleColor.White;
            }

        }
        public static void JoinMatch(Socket socket, string userNickname, int role)
        {
            Frame frameRequest = new Frame(ActionType.JoinMatch, userNickname, role);
            FrameConnection.Send(socket, frameRequest);
            Frame frameResponse = FrameConnection.Receive(socket);
            if (frameResponse.Data.Equals("OK"))
            {
                isInActiveMatch = true;
                Console.WriteLine("Se ha Unido a la partida");
            }
            else
            {
                Console.WriteLine(frameResponse.Data);
            }
        }

        public static void MovePlayer(Socket socket, string userNickname, int movementDirection)
        {
            if (isInActiveMatch == true)
            {

            
            Frame frameRequest = new Frame(ActionType.MovePlayer, userNickname, movementDirection);
            FrameConnection.Send(socket, frameRequest);
            Frame frameResponse = FrameConnection.Receive(socket);
            if (frameResponse.Data.Equals("Estas muerto"))
            {
                isInActiveMatch = false;
                Console.WriteLine(frameResponse.Data);
            }
            else if (frameResponse.Data.Contains("Partida terminada"))
            {
                isInActiveMatch = false;
                Console.WriteLine(frameResponse.Data);
            }
            else
                Console.WriteLine(frameResponse.Data);
            }
        }

        public static void AttackPlayer(Socket socket, string userNickname, int attackDirection)
        {
            Frame frameRequest = new Frame(ActionType.AttackPlayer, userNickname, attackDirection);
            FrameConnection.Send(socket, frameRequest);
            Frame frameResponse = FrameConnection.Receive(socket);
            if (frameResponse.Data.Equals("Estas muerto"))
            {
                isInActiveMatch = false;
                Console.WriteLine(frameResponse.Data);
            }
            else if (frameResponse.Data.Contains("Partida terminada"))
            {
                isInActiveMatch = false;
                Console.WriteLine(frameResponse.Data);
            }
            else
                Console.WriteLine(frameResponse.Data);
        }

        private static bool IsEmpty(string[] listFiles)
        {
            return listFiles == null || listFiles.Length == 0;
        }

        private static string[] GetListFormatted(string data)
        {
            string[] list = null;
            if (data != null)
            {
                string items = data;
                if (!items.Equals(""))
                {
                    list = items.Split(';');
                }
            }
            return list;
        }

        public static void Exit(Socket socket, string user)
        {
            Frame frameRequest = new Frame(ActionType.Exit, user);
            FrameConnection.Send(socket, frameRequest);
        }

        public static void ListRegisteredUsers(Socket socket)
        {
            Frame frameRequest = new Frame(ActionType.ListRegisteredUsers, "");
            FrameConnection.Send(socket, frameRequest);
            Frame frameResponse = FrameConnection.Receive(socket);
            string[] listUsers = GetListFormatted(frameResponse.Data);
            if (!IsEmpty(listUsers))
            {
                System.Console.WriteLine("Usuarios Registrados: ");
                foreach (string user in listUsers)
                {
                    System.Console.WriteLine(user);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("No hay usuarios registrados");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }



        /* public void CreatePlayer(string nickname)
         {
             string message = MessageDecoder.EncodeMessage(MessageDecoder.RequestMessage, ActionType.NewPlayer, nickname);

             clientProtocol.SendRequestMessage(message);
         }*/
    }
}

