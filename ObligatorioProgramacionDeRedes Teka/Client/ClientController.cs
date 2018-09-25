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
        private ClientProtocol clientProtocol;

        public void Connect()
        {
            clientProtocol = new ClientProtocol();
            clientProtocol.Connect();
        }

        public void ListConnectedUsers()
        {
            Frame frameRequest = new Frame(ActionType.ListConnectedUsers, "");
            clientProtocol.Send(frameRequest);
            Frame frameResponse = clientProtocol.Receive();
            string[] listUsers = GetListFormatted(frameResponse.Data);
            if (!IsEmpty(listUsers))
            {
                System.Console.WriteLine("Usuarios: ");
                foreach (string user in listUsers)
                {
                    System.Console.WriteLine(user);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("No hay usuarios.");
                Console.ForegroundColor = ConsoleColor.White;
            }

        }

        internal void Close()
        {
            clientProtocol.Close();
        }

        public string ReceiveConnectionConfirmation()
        {
            Frame responseFrame = clientProtocol.Receive();
            return responseFrame.Data;
        }

        public void ConnectUser(string user)
        {
            Frame frameRequest = new Frame(ActionType.ConnectToServer, user);
            clientProtocol.Send(frameRequest);
            //Frame frameResponse = FrameConnection.Receive();

            //string isConnected = Encoding.ASCII.GetString(frameResponse.Data, 0, frameResponse.Data.Length);
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
                if (!data.Equals(""))
                {
                    list = data.Split(';');
                }
            }
            return list;
        }

        public void Exit(string user)
        {
            Frame frameRequest = new Frame(ActionType.Exit, user);
            clientProtocol.Send(frameRequest);
            //FrameConnection.Send(socket, frameRequest);
        }

       /* public void CreatePlayer(string nickname)
        {
            string message = MessageDecoder.EncodeMessage(MessageDecoder.RequestMessage, ActionType.NewPlayer, nickname);
            
            clientProtocol.SendRequestMessage(message);
        }*/
    }
}

