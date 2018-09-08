using System;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;

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

            _server.clientSockets[fromSocket] = masterNetworkPlayer;
            _server.clientSockets[fromSocket].id = fromId;

            

            Console.WriteLine("Users info: " + _server.clientSockets[fromSocket].id + ", " + _server.clientSockets[fromSocket].username);
        }
    }
}