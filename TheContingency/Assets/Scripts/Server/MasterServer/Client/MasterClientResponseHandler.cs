using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;

namespace SNetwork.Client
{
	public class MasterClientResponseHandler
    {

        private MasterClient _client;

        public MasterClientResponseHandler(MasterClient client)
        {
            _client = client;
        }

        public void Initialize()
        {
            ResponseManager.instance.AddServerResponse(Response1, 1);
            ResponseManager.instance.AddServerResponse(Response4, 4);
            ResponseManager.instance.AddServerResponse(Response5, 5);
            ResponseManager.instance.AddServerResponse(Response6, 6);
            ResponseManager.instance.AddServerResponse(Response7, 7);
            ResponseManager.instance.AddServerResponse(Response9, 9);
            ResponseManager.instance.AddServerResponse(Response14, 14);
            ResponseManager.instance.AddServerResponse(Response70, 70);
            ResponseManager.instance.AddServerResponse(Response71, 71);
            ResponseManager.instance.AddServerResponse(Response72, 72);
            ResponseManager.instance.AddServerResponse(Response73, 73);
        }

        public void Response1(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            _client.serverData = ByteParser.ConvertDataToKeyValuePairs(responseBytes);
        }

        public void Response4(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a command response: " + ByteParser.ConvertToASCII(responseBytes));
        }

        public void Response5(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a invalid request");
        }

        public void Response6(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a failure");
        }

        public void Response7(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Recieved a message: " + ByteParser.ConvertToASCII(responseBytes));
        }

        public void Response9(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Logging.CreateLog("Hello: " + ByteParser.ConvertToASCII(responseBytes));
            _client.ourId = int.Parse(ByteParser.ConvertToASCII(responseBytes));
            Logging.CreateLog("Recieved the id: " + _client.ourId);
        }

        public void Response14(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            _client.networkPlayers = ByteParser.ConvertToNetworkPlayers(responseBytes);
        }

        public void Response70(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            _client.room = ByteParser.ConvertDataToRoom(responseBytes);
        }

        public void Response71(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            _client.invites = ByteParser.ConvertDataToInvites(responseBytes).ToArray();
        }

        public void Response72(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            _client.match = ByteParser.ConvertDataToMatch(responseBytes);
        }

        public void Response73(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            // Recieved match callback!
            Logging.CreateLog("Recieved Match!");
            _client.match = ByteParser.ConvertDataToMatch(responseBytes);
            if(MasterClientManager.instance.onMatchFound != null)
                MasterClientManager.instance.onMatchFound.Invoke();
        }
    }
}
