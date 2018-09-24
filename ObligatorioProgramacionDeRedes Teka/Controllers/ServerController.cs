
using DataManager;
using Protocol;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class ServerController
    {
        public static string Connect(Frame frame, List<User> users)
        {
            string receive = Encoding.ASCII.GetString(frame.Data, 0, frame.DataLength);
            string nickname = receive;
            User userAux = new User(nickname);
            foreach (var user in users)
            {
                if (user != null && user.Equals(userAux))
                {
                    return "REPEAT";
                }
            }
            return "OK";
        }

        public static void ListUsers(Socket socket, List<User> users)
        {
            string response = "";
            foreach (var user in users)
            {
                if (user != null)
                {
                    response += user.Nickname + ";";
                }
            }

            Frame frame = new Frame(ActionType.ListConnectedUsers, Encoding.ASCII.GetBytes(response));
            FrameConnection.Send(socket, frame);
        }

        public static void Exit(Socket socket, string user, UsersRepository lists)
        {
           /* User userToClose = lists.GetUserByName(user);
            Frame frame = new Frame(ActionType.Exit, Encoding.ASCII.GetBytes(""));
            FrameConnection.Send(socket, frame);*/
        }
    }
}
