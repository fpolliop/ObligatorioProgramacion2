
using DataManager;
using Game;
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
        private static SlasherMatch match;

        public static string Connect(Frame frame, List<User> users)
        {
            string receive = frame.Data;
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

        public static SlasherMatch GetMatch()
        {
            if (match != null)
            {
                match = new SlasherMatch();
            }
            return match;
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

            Frame frame = new Frame(ActionType.ListConnectedUsers, response);
            FrameConnection.Send(socket, frame);
        }

        public static void Exit(Socket socket, string user, UsersRepository lists)
        {
           /* User userToClose = lists.GetUserByName(user);
            Frame frame = new Frame(ActionType.Exit, Encoding.ASCII.GetBytes(""));
            FrameConnection.Send(socket, frame);*/
        }

        public static void JoinPlayerToMatch(Socket socket, User user, Role role)
        {
            Player newPlayer = new Player(user.Nickname, role);
            match = GetMatch();
            match.AddPlayer(newPlayer);
            if (!match.hasStarted)
            {
                match.StartMatch();
            }
            Frame frame = new Frame(ActionType.JoinMatch, "OK");
            FrameConnection.Send(socket, frame);
        }
    }
}
