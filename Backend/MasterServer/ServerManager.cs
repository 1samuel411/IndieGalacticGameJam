﻿using System;
using System.Linq;
using System.Net.Sockets;

namespace SNetwork.Server
{
    public class ServerManager
    {
        public static ServerManager instance;
        private ServerResponseHandler _serverResponseHandler;
        public Server server;

        public static void Main(string[] args)
        {
            var serverManager = new ServerManager();
            serverManager.Start();
        }

        private void Start()
        {
            ResponseManager.instance.Clear();

            server = new Server();
            _serverResponseHandler = new ServerResponseHandler(server);
            _serverResponseHandler.Initialize();
            InitializeCommands();

            instance = this;

            //Create("127.0.0.1", 1525, 50000, 1000, "", "NA");
            //ListenForCommands();
            //return;

            Console.WriteLine("Enter the server's ip");
            var ip = Console.ReadLine();
            Console.WriteLine("Enter the server's port");
            var port = Console.ReadLine();

            var bufferSize = 256000;

            Create(ip, int.Parse(port), bufferSize);
            ListenForCommands();
        }

        private void ListenForCommands()
        {
            while (true)
            {
                var command = Console.ReadLine();
                if (command.Equals("Help"))
                    Console.WriteLine("The authorities have been alerted!");

                if (command.Equals("ListRooms"))
                {
                    Console.WriteLine("-------------------------------------");

                    for (int i = 0; i < server.rooms.Count; i++)
                    {
                        Console.WriteLine("Room: " + server.rooms[i].roomId);
                        Console.WriteLine("        Started: " + server.rooms[i].startedGame);
                        Console.WriteLine("        Ended: " + server.rooms[i].game.ended);
                        Console.WriteLine("        Input A: " + server.rooms[i].game.inputA);
                        Console.WriteLine("        Input B: " + server.rooms[i].game.inputB);
                        Console.WriteLine("        Users");
                        for (int x = 0; x < server.rooms[i].usersInRoomIds.Count; x++)
                        {
                            Console.WriteLine("            User " + (x + 1) + ": " + server.rooms[i].usersInRoomIds[x].ToString());
                        }
                    }

                    Console.WriteLine("-------------------------------------");
                }

                if (command.Equals("ListUsers"))
                {
                    Console.WriteLine("-------------------------------------");

                    for (int i = 0; i < server.clientSockets.Count; i++)
                    {
                        Console.WriteLine("User: " + server.clientSockets.Values.ElementAt(i).id);
                        Console.WriteLine("    Username: " + server.clientSockets.Values.ElementAt(i).username);
                    }

                    Console.WriteLine("-------------------------------------");
                }

                if (command.Equals("Stop"))
                {
                    Console.WriteLine("Stopping");

                    for (int i = 0; i < server.clientSockets.Count; i++)
                    {
                        server.RemoveSocket(server.clientSockets.Keys.ElementAt(i));
                    }

                    Close();
                }
            }
        }

        public void Create(string ip = "127.0.0.1", int port = 100, int bufferSize = 5000000)
        {
            server.SetupServer(ip, port, bufferSize);
        }

        public void Close()
        {
            server.CloseServer();
        }

        private void InitializeCommands()
        {
            CommandHandler.AddCommand(new Command("time", TimeCommand));
            CommandHandler.AddCommand(new Command("setname", SetName));
            CommandHandler.AddCommand(new Command("kick", Kick));
            CommandHandler.AddCommand(new Command("leave", Leave));
        }

        private void TimeCommand(string text, Socket fromSocket, int fromId)
        {
            Messaging.instance.SendCommandResponse(DateTime.UtcNow.ToString(), fromId, 0, 0, server.clientSockets);
        }

        private void SetName(string newName, Socket fromSocket, int fromId)
        {
            server.clientSockets[fromSocket].username = newName;
            Console.WriteLine("[SNetworking] Setting MasterNetworkPlayer: " + server.clientSockets[fromSocket].id +
                              ", NetworkPlayername to: " + server.clientSockets[fromSocket].username);
            Messaging.instance.SendCommandResponse("[Success]", server.clientSockets[fromSocket].id, 0, 0,
                server.clientSockets);
        }

        private void Kick(string name, Socket fromSocket, int fromId)
        {
            var NetworkPlayerSelected = from x in server.clientSockets where x.Value.username == name select x;

            if (NetworkPlayerSelected.Any())
                Console.WriteLine("[SNetworking] Could not find MasterNetworkPlayer with the name: " + name);
            else
                for (var i = 0; i < NetworkPlayerSelected.Count(); i++)
                {
                    Console.WriteLine("[SNetworking] Kicking MasterNetworkPlayer: " +
                                      NetworkPlayerSelected.ElementAt(i).Value.id + ", with the NetworkPlayername: " +
                                      name);
                    Messaging.instance.SendCommandResponse("[Kicked]", 2, 0, 0, server.clientSockets);

                    server.RemoveSocket(NetworkPlayerSelected.ElementAt(i).Key);
                    NetworkPlayerSelected.ElementAt(i).Key.Shutdown(SocketShutdown.Both);
                    NetworkPlayerSelected.ElementAt(i).Key.Close();
                }
        }

        private void Leave(string text, Socket fromSocket, int fromId)
        {
            Console.WriteLine(
                "[SNetworking] MasterNetworkPlayer: " + server.clientSockets[fromSocket].id + " has left.");
            server.RemoveSocket(fromSocket);
            try
            {
                fromSocket.Disconnect(false);
                fromSocket.Shutdown(SocketShutdown.Both);
                fromSocket.Close();
            }
            catch (ObjectDisposedException e)
            {
                
            }
        }
    }
}