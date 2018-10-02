﻿using Controllers;
using DataManager;
//using ObligatorioProgramacionDeRedes;
using Protocol;
using Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public class Program
    {
        private static ServerProtocol serverProtocol;
        private static int clientCount = 0;
        private static UsersRepository lists;
       // private static bool serverIsActive = true;
       // private static bool socketIsClosed = false;

        static void Main(string[] args)
        {
            Console.Title = "Server";
            lists = new UsersRepository();
            serverProtocol = new ServerProtocol();
            Socket server = serverProtocol.StartServer();
            bool serverIsActive = true;
            try
            {
                while (serverIsActive)
                {
                    Console.WriteLine("Esperando conexion con el cliente...");
                    Socket client = server.Accept();
                    Socket clientNotify = server.Accept();
                    Thread thread = new Thread(() => ProcessClient(client, clientNotify));
                    thread.Start();
                }
            }
            catch
            {
                Console.Write("Ha ocurrido algo");
            }
        }

        private static void ProcessClient(Socket client, Socket clientNotify)
        {
            //Console.WriteLine("Conectado el cliente " + clientCount);
            Frame frameReceived = null;
            string userNickname = "";
            bool socketIsClosed = false;
            Image userAvatar = null;

            while (!socketIsClosed)
            {
                try
                {
                    frameReceived = FrameConnection.Receive(client);
                    userNickname = frameReceived.GetUserNickname();
                    userAvatar = FrameConnection.ReceiveAvatar(client);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    socketIsClosed = true;
                }

                try
                {
                    
                    switch (frameReceived.Action)
                    {

                        case ActionType.ConnectToServer:
                            

                            string response = ServerController.Connect(frameReceived, lists.GetUsers());
                            if (response.Equals("OK"))
                            {
                                AddUserInList(userNickname, userAvatar, clientNotify);
                                clientCount++;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Conectado el cliente: " + userNickname);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else if (response.Equals("EXISTENT"))
                            {
                                ConnectUser(userNickname, clientNotify);

                                clientCount++;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Conectado el cliente: " + userNickname);
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                  
                            Frame frame = new Frame(ActionType.ConnectToServer, response);
                            FrameConnection.Send(client, frame);
                            break;
                        case ActionType.ListConnectedUsers:
                            ServerController.ListConnectedUsers(client, lists.GetUsers());
                            break;
                        case ActionType.ListRegisteredUsers:
                            ServerController.ListRegisteredUsers(client, lists.GetUsers());
                            break;
                        case ActionType.JoinMatch:
                            User user = lists.GetUserByName(userNickname);
                            Role role = frameReceived.GetUserRole();
                            ServerController.JoinPlayerToMatch(client, user, role);
                            break;
                        case ActionType.MovePlayer:
                            User player = lists.GetUserByName(userNickname);
                            PlayerGameAction gameAction = frameReceived.GetPlayerGameAction();
                            ServerController.MovePlayer(client, player, gameAction, lists);
                            break;
                        case ActionType.AttackPlayer:
                            User attacker = lists.GetUserByName(userNickname);
                            PlayerGameAction action = frameReceived.GetPlayerGameAction();
                            ServerController.AttackPlayer(client, attacker, action, lists);
                            break;
                        case ActionType.Exit:
                            ServerController.Exit(client, userNickname, lists);
                            userNickname = frameReceived.Data;
                            DisconnectUser(userNickname);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Desconectado el cliente: " + userNickname);
                            Console.ForegroundColor = ConsoleColor.White;
                            socketIsClosed = true;
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Ha sucedido un error inesperado");

                    if (!userNickname.Equals(""))
                    {
                        DisconnectUser(userNickname);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Desconectado el cliente: " + userNickname);
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    socketIsClosed = true;
                }
            }
            // string messageReceived = serverConnection.ReceiveMessage();
            //string[] clientRequest = MessageDecoder.DecodeMessage(messageReceived);
            // serverController.HandleClientRequest(clientRequest);
           // client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private static void ConnectUser(string nickname, Socket clientNotify)
        {
            User user = lists.GetUserByName(nickname);
            if (user != null)
            {
                user.IsConnected = true;
                user.SocketNotify = clientNotify;
            }
        }

        private static void AddUserInList(string nickname, Image userAvatar, Socket socketNotify)
        {
            User user = new User(nickname, userAvatar, socketNotify);
            lists.AddUser(user);
        }

        private static void DisconnectUser(string nickname)
        {
            User user = lists.GetUserByName(nickname);
            if (user != null)
            {
                user.IsConnected = false;
                
            }
            else
            {
                Console.WriteLine("Error!, No se ha podido eliminar el user: " + nickname);
            }
        }
    }
}
