using System;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ServerModels;

namespace SNetwork.Server
{
    public class ServerResponseHandler
    {
        private readonly Server _server;

        public ServerResponseHandler(Server server)
        {
            _server = server;
        }

        public void Initialize()
        {
            ResponseManager.instance.Clear();
            ResponseManager.instance.AddServerResponse(Response21, 21);
            ResponseManager.instance.AddServerResponse(Response20, 20);
            ResponseManager.instance.AddServerResponse(Response12, 12);
            ResponseManager.instance.AddServerResponse(Response3, 3);
            ResponseManager.instance.AddServerResponse(Response2, 2);

            ResponseManager.instance.AddServerResponse(Response50, 50);

            ResponseManager.instance.AddServerResponse(Response72, 72);
            ResponseManager.instance.AddServerResponse(Response73, 73);
            ResponseManager.instance.AddServerResponse(Response74, 74);
            ResponseManager.instance.AddServerResponse(Response75, 75);
            ResponseManager.instance.AddServerResponse(Response76, 76);
            ResponseManager.instance.AddServerResponse(Response77, 77);

            ResponseManager.instance.AddServerResponse(Response90, 90);

            ResponseManager.instance.AddServerResponse(Response200, 200);
            ResponseManager.instance.AddServerResponse(Response201, 201);
            ResponseManager.instance.AddServerResponse(Response202, 202);
            ResponseManager.instance.AddServerResponse(Response203, 203);
        }

        public void Response90(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            int newPlayMode = int.Parse(ByteParser.ConvertToASCII(responseBytes));
            ServerManager.instance.server.clientSockets[fromSocket].playMode = newPlayMode;
        }

        public void Response200(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            int newPort = int.Parse(ByteParser.ConvertToASCII(responseBytes));
            Console.WriteLine("Recieved match's port: " + fromId + ": " + newPort);
            ServerManager.instance.server.matchSockets[fromSocket].port = newPort;
        }

        public void Response201(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            string newIp = (ByteParser.ConvertToASCII(responseBytes));
            Console.WriteLine("Recieved match's ip: " + fromId + ": " + newIp);
            ServerManager.instance.server.matchSockets[fromSocket].ip = newIp;

            Console.WriteLine("Recieved match ready to be joined: " + fromId);
            ServerManager.instance.server.matchSockets[fromSocket].serverRunning = true;
            ServerManager.instance.server.matchSockets[fromSocket].open = true;
            ServerManager.instance.server.matchSockets[fromSocket].startTime = DateTime.UtcNow;
        }

        public void Response202(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            //Console.WriteLine("Recieved match ready to be joined: " + fromId);
            ServerManager.instance.server.matchSockets[fromSocket].serverRunning = true;
            ServerManager.instance.server.matchSockets[fromSocket].open = true;
            ServerManager.instance.server.matchSockets[fromSocket].startTime = DateTime.UtcNow;
        }

        public void Response203(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            string userPlayfabIdLeft = ByteParser.ConvertToASCII(responseBytes);
            Console.WriteLine("User has left match: " + userPlayfabIdLeft + ", from match: " + ServerManager.instance.server.matchSockets[fromSocket].id);
            ServerManager.instance.server.LeaveMatch(userPlayfabIdLeft);
        }

        public void Response21(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 21: " + fromId + ": " + ByteParser.ConvertToASCII(responseBytes));
            CommandHandler.RunCommand(ByteParser.ConvertToASCII(responseBytes), fromSocket, fromId);
        }

        public void Response20(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 20: " + ByteParser.ConvertToASCII(responseBytes));
        }

        public void Response12(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 12: " + fromId + ": " + responseBytes.Length);
        }

        public void Response3(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 3: " + fromId + ": " + responseBytes.Length);
            _server.SetUserData(fromId, ByteParser.ConvertDataToKeyValuePair(responseBytes));
        }

        public void Response2(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 2: " + fromId + ": " + responseBytes.Length);
            _server.SetServerData(ByteParser.ConvertDataToKeyValuePair(responseBytes));
        }

        public void Response50(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 50: " + fromId + ": " + responseBytes.Length);
            MasterNetworkPlayer masterNetworkPlayer = ByteParser.ConvertToNetworkPlayer(responseBytes);

            // Check it doesn't exist
            MasterNetworkPlayer check = _server.clientSockets.Values.FirstOrDefault(x => x.playfabId == masterNetworkPlayer.playfabId);
            if (check != null)
            {
                // Exists
                Console.WriteLine("[SNetworking] This user already is loggged in");
                Messaging.instance.SendInfoMessage(fromSocket, "Already Logged In", 0);
                _server.RemoveSocket(fromSocket);
                return;
            }

            ServerManager.instance.server.SetLoggedIn(masterNetworkPlayer.playfabId);
            _server.clientSockets[fromSocket] = masterNetworkPlayer;
            _server.clientSockets[fromSocket].id = fromId;

            

            Console.WriteLine("Users info: " + _server.clientSockets[fromSocket].id + ", " + _server.clientSockets[fromSocket].username + ", " + _server.clientSockets[fromSocket].playfabId);
        }

        public void Response72(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            // Recieve playfabId to send invite to
            string playfabTarget = ByteParser.ConvertToASCII(responseBytes);
            Console.WriteLine("Recieved invite request to: " + playfabTarget);
            ServerManager.instance.server.InviteToRoom(ServerManager.instance.server.clientSockets[fromSocket].playfabId, playfabTarget);
        }

        public void Response73(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            // Recieve invite Id to accept
            int inviteTarget = int.Parse(ByteParser.ConvertToASCII(responseBytes));
            Console.WriteLine("Recieved accept invite request to: " + inviteTarget);
            ServerManager.instance.server.AcceptInvite(inviteTarget);
        }

        public void Response74(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            // Recieve invite Id to decline
            int inviteTarget = int.Parse(ByteParser.ConvertToASCII(responseBytes));
            Console.WriteLine("Recieved decline invite request to: " + inviteTarget);
            ServerManager.instance.server.DeclineInvite(inviteTarget);
        }

        public void Response75(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            // Leave
            Console.WriteLine("Recieved leave request from: " + fromId);
            ServerManager.instance.server.LeaveRoom(fromSocket);
        }

        public void Response76(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            // Leave
            Console.WriteLine("Recieved match make request from: " + fromId);
            ServerManager.instance.server.matchMaking.AddToMatchMaking(fromSocket);
        }

        public void Response77(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            // Leave
            Console.WriteLine("Recieved leave match making request from: " + fromId);
            ServerManager.instance.server.matchMaking.RemoveFromMatchMaking(fromSocket);
        }
    }
}