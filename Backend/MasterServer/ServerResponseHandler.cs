using System;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;
using MasterServerProj.DataModels;

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

            ResponseManager.instance.AddServerResponse(Response51, 51);
            ResponseManager.instance.AddServerResponse(Response52, 52);
            ResponseManager.instance.AddServerResponse(Response53, 53);

            ResponseManager.instance.AddServerResponse(Response80, 80);

            ResponseManager.instance.AddServerResponse(Response88, 88);
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

        public void Response51(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 51: " + fromId + ": " + responseBytes.Length);
            int idToJoin = int.Parse(ByteParser.ConvertToASCII(responseBytes));
            _server.JoinRoom(fromSocket, idToJoin.ToString());
        }

        public void Response52(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 52: " + fromId + ": " + responseBytes.Length);
            _server.CreateRoom(fromSocket);
        }

        public void Response53(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 53: " + fromId + ": " + responseBytes.Length);
            _server.LeaveRoom(fromSocket);
        }

        public void Response80(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 80: " + fromId + ": " + responseBytes.Length);
            _server.ToggleReady(fromSocket);
        }

        public void Response88(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 88: " + fromId + ": " + responseBytes.Length);
            ControllerInput input = ByteParser.ConvertDataToInput(responseBytes);
            Console.WriteLine(input.attitude + ", " + input.pressure + ", " + input.speed);
            _server.SetInput(fromSocket, input);
        }
    }
}