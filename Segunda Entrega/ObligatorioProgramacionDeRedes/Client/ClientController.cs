using Protocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using static Protocol.Frame;

namespace Client
{
    public class ClientController
    {
        public bool isInActiveMatch;

        public ClientController()
        {
            isInActiveMatch = false;
        }

        public void ListConnectedUsers(Socket socket)
        {
            Frame frameRequest = new Frame(Command.ListConnectedUsers, "");
            FrameConnection.Send(socket, frameRequest);
            Frame frameResponse = FrameConnection.Receive(socket);
            string[] listUsers = GetListFormatted(frameResponse.Data);
            if (!IsEmpty(listUsers))
            {
                Console.WriteLine("Usuarios Conectados: ");
                foreach (string user in listUsers)
                {
                    Console.WriteLine(user);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No hay usuarios conectados");
                Console.ForegroundColor = ConsoleColor.White;
            }

        }
        public void JoinMatch(Socket socket, string userNickname, int role)
        {
            Frame frameRequest = new Frame(Command.JoinMatch, userNickname, role);
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

        public void MovePlayer(Socket socket, string userNickname, int movementDirection)
        {
            if (isInActiveMatch == true)
            {


                Frame frameRequest = new Frame(Command.MovePlayer, userNickname, movementDirection);
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

        public void AttackPlayer(Socket socket, string userNickname, int attackDirection)
        {
            Frame frameRequest = new Frame(Command.AttackPlayer, userNickname, attackDirection);
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

        private bool IsEmpty(string[] listFiles)
        {
            return listFiles == null || listFiles.Length == 0;
        }

        private string[] GetListFormatted(string data)
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

        public void Exit(Socket socket, string user)
        {
            Frame frameRequest = new Frame(Command.Exit, user);
            FrameConnection.Send(socket, frameRequest);
        }

        public void ListRegisteredUsers(Socket socket)
        {
            Frame frameRequest = new Frame(Command.ListRegisteredUsers, "");
            FrameConnection.Send(socket, frameRequest);
            Frame frameResponse = FrameConnection.Receive(socket);
            string[] listUsers = GetListFormatted(frameResponse.Data);
            if (!IsEmpty(listUsers))
            {
                Console.WriteLine("Usuarios Registrados: ");
                foreach (string user in listUsers)
                {
                    Console.WriteLine(user);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No hay usuarios registrados");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void ViewLog(Socket socket)
        {
            Frame frameRequest = new Frame(Command.ViewLog, "");
            FrameConnection.Send(socket, frameRequest);
            Frame frameResponse = FrameConnection.Receive(socket);
            string[] logLines = GetListFormatted(frameResponse.Data);
            if (!IsEmpty(logLines))
            {
                Console.WriteLine("Log de Ultima Partida: ");
                foreach (string line in logLines)
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se encontro el log de la ultima partida.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        
    }
}
