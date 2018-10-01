
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
                    if (user.IsConnected)
                    {
                        return "REPEAT";
                    }
                    else
                    {
                        return "EXISTENT";
                    } 
                }
            }
            return "OK";
        }

        public static SlasherMatch GetMatch()
        {
            if (match == null)
            {
                match = new SlasherMatch();
            }
            return match;
        }

        public static void ListConnectedUsers(Socket socket, List<User> users)
        {
            string response = "";
            foreach (var user in users)
            {
                if (user != null && user.IsConnected == true)
                {
                    response += user.Nickname + ";";
                }
            }

            Frame frame = new Frame(ActionType.ListConnectedUsers, response);
            FrameConnection.Send(socket, frame);
        }

        public static void ListRegisteredUsers(Socket socket, List<User> users)
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
            try
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
            } catch (Exception ex)
            {
                Frame frame = new Frame(ActionType.JoinMatch, ex.Message);
                FrameConnection.Send(socket, frame);
            }
        }

        public static void MovePlayer(Socket socket, User user, PlayerGameAction gameAction)
        {
            try
            {
                Player player = match.GetPlayer(user);
                match = GetMatch();
                string response = match.MovePlayer(player, gameAction);
                
                Frame frame = new Frame(ActionType.MovePlayer, response);
                FrameConnection.Send(socket, frame);
            }
            catch (Exception ex)
            {
                Frame frame = new Frame(ActionType.MovePlayer, ex.Message);
                FrameConnection.Send(socket, frame);
            }
        }

        public static void AttackPlayer(Socket socket, User user, PlayerGameAction gameAction)
        {
            try
            {
                Player player = match.GetPlayer(user);
                match = GetMatch();
                string response = match.AttackPlayer(player, gameAction);

                Frame frame = new Frame(ActionType.AttackPlayer, response);
                FrameConnection.Send(socket, frame);
            }
            catch (Exception ex)
            {
                Frame frame = new Frame(ActionType.AttackPlayer, ex.Message);
                FrameConnection.Send(socket, frame);
            }
        }

    }
}
