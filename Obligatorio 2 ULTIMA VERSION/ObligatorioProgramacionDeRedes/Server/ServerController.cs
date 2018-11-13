using Entities;
using Protocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using static Protocol.Frame;
using Log;

namespace Server
{
    public class ServerController
    {
        private Match match;
        private bool activeMatch;
        private Repository repository;

        public ServerController(Repository repository)
        {
            match = null;
            activeMatch = false;
            this.repository = repository;
            SlasherLog.Start();
        }

        public string Connect(Frame frame, List<User> users)
        {
            string received = frame.Data;
            string nickname = received;
            foreach (User user in users)
            {
                if (user != null && user.Nickname.Equals(nickname))
                {
                    if (user.IsConnected)
                    {
                        return "REPEATED";
                    }
                    else
                    {
                        return "EXISTENT";
                    }
                }
            }
            return "OK";
        }

        public Match GetMatch()
        {
            if (match == null)
            {
                match = new Match(repository);
            }
            return match;
        }

        public void ListConnectedUsers(Socket socket, List<User> users)
        {
            if (!activeMatch)
            {
                string response = "";
                foreach (User user in users)
                {
                    if (user != null && user.IsConnected == true)
                    {
                        response += user.Nickname + ";";
                    }
                }

                Frame frame = new Frame(Command.ListConnectedUsers, response);
                FrameConnection.Send(socket, frame);
            }
            else
            {
                string response = "No se puede realizar pedidos al servidor cuando hay una partida activa";
                Frame frame = new Frame(Command.ListConnectedUsers, response);
                FrameConnection.Send(socket, frame);
            }
        }

        public void ListRegisteredUsers(Socket socket, List<User> users)
        {
            if (!activeMatch)
            {
                string response = "";
                foreach (User user in users)
                {
                    if (user != null)
                    {
                        response += user.Nickname + ";";
                    }
                }

                Frame frame = new Frame(Command.ListRegisteredUsers, response);
                FrameConnection.Send(socket, frame);
            }
            else
            {
                string response = "No se puede realizar pedidos al servidor cuando hay una partida activa";
                Frame frame = new Frame(Command.ListRegisteredUsers, response);
                FrameConnection.Send(socket, frame);
            }
        }

        public void Exit(Socket socket, string nickname, Repository lists)
        {
            User userToClose = lists.GetUserByNickname(nickname);
            Frame frame = new Frame(Command.Exit, "");
            FrameConnection.Send(userToClose.SocketNotify, frame);
        }

        public void JoinPlayerToMatch(Socket socket, User user, Role role)
        {
            try
            {
                Player newPlayer = new Player(user.Nickname, role);
                match = GetMatch();
                if (match.HasFinished)
                    match = new Match(repository);
                match.AddPlayer(newPlayer);
                if (!match.HasStarted)
                {
                    match.StartMatch();
                    activeMatch = true;
                }
                Frame frame = new Frame(Command.JoinMatch, "OK");
                FrameConnection.Send(socket, frame);
            }
            catch (Exception ex)
            {
                Frame frame = new Frame(Command.JoinMatch, ex.Message);
                FrameConnection.Send(socket, frame);
            }
        }

        public void MovePlayer(Socket socket, User user, PlayerGameAction gameAction, Repository usersList)
        {
            try
            {
                Player player = match.GetPlayer(user.Nickname);
                string response = match.MovePlayer(player, gameAction);

                Frame frame = new Frame(Command.MovePlayer, response);
                FrameConnection.Send(socket, frame);
            }
            catch (Exception ex)
            {
                if (!match.HasFinished)
                {
                    Frame frame = new Frame(Command.MovePlayer, ex.Message);
                    FrameConnection.Send(socket, frame);
                }
                else
                {
                    NotifyAllPlayers(usersList, ex.Message);
                    Frame frame = new Frame(Command.MovePlayer, "Partida terminada"); //borrar mensaje?
                    FrameConnection.Send(socket, frame);
                }
            }
        }

        private void NotifyAllPlayers(Repository users, string response)
        {
            activeMatch = false;
            List<User> listUsers = users.GetUsers();
            foreach (User user in listUsers)
            {
                if (user.IsConnected)
                    NotifyUser(user.Nickname, users, response);
            }
        }

        private void NotifyUser(string user, Repository list, string response)
        {
            User userNotify = list.GetUserByNickname(user);
            string messageToNotify = response;
            Frame frame = new Frame(Command.MovePlayer, messageToNotify);
            Socket socketToNotify = userNotify.SocketNotify;
            FrameConnection.Send(socketToNotify, frame);
        }

        public void AttackPlayer(Socket socket, User user, PlayerGameAction gameAction, Repository usersList)
        {
            try
            {
                Player player = match.GetPlayer(user.Nickname);
                string response = match.AttackPlayer(player, gameAction);

                Frame frame = new Frame(Command.AttackPlayer, response);
                FrameConnection.Send(socket, frame);
            }
            catch (Exception ex)
            {
                if (!match.HasFinished)
                {
                    Frame frame = new Frame(Command.AttackPlayer, ex.Message);
                    FrameConnection.Send(socket, frame);
                }
                else
                {
                    NotifyAllPlayers(usersList, ex.Message);
                    Frame frame = new Frame(Command.AttackPlayer, "Partida terminada");
                    FrameConnection.Send(socket, frame);
                }
            }
        }

        public void RemoveFromActiveMatch(string nickname)
        {
            if (match != null)
            {
                Player player = match.GetPlayer(nickname);
                if (player != null)
                {
                    match.RemovePlayerFromActiveMatch(player);
                    if (match.PlayersInMatch.Count == 1)
                    {
                        match.HasFinished = true;
                        activeMatch = false;
                    }
                }
            }
        }

        internal List<string> GetLog()
        {
            return SlasherLog.GetLastMatchLog();
        }
    }
}
