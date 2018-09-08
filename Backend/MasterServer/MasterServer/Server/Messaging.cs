using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using SNetwork.Server;

namespace SNetwork
{
    public class Messaging
    {
        private static Messaging _instance;

        public static Messaging instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Messaging();
                return _instance;
            }
        }

        // Header: 73
        public void SendMatchFound(Match match, int sendCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            var data = ByteParser.ConvertMatchToData(match);
            SendFinal(data, 73, sendCode, 0, 0, sockets);
        }

        // Header: 72
        public void SendMatch(Match match, int sendCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            var data = ByteParser.ConvertMatchToData(match);
            SendFinal(data, 72, sendCode, 0, 0, sockets);
        }

        // Header: 71
        public void SendInvites(List<Invite> invites, int sendCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            var data = ByteParser.ConvertInvitesToData(invites);
            SendFinal(data, 71, sendCode, 0, 0, sockets);
        }

        // Header: 70
        public void SendRoom(Room room, int sendCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            var data = ByteParser.ConvertRoomToData(room);
            SendFinal(data, 70, sendCode, 0, 0, sockets);
        }

        // Header: 21
        public void SendCommand(string text, int sendCode, int fromCode, int customCode,
            Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            var data = ByteParser.ConvertObjectToBytes(text);
            SendFinal(data, 21, sendCode, fromCode, customCode, sockets);
        }

        // Header: 20
        public void SendString(string text, int sendCode, int fromCode, int customCode,
            Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            var data = ByteParser.ConvertObjectToBytes(text);
            SendFinal(data, 20, sendCode, fromCode, customCode, sockets);
        }

        // Header: 14
        public void SendNetworkPlayers(byte[] data, Dictionary<Socket, MasterNetworkPlayer> sockets, int sendcode = 2)
        {
            SendFinal(data, 14, 2, 0, 0, sockets);
        }

        // Header: 12
        public void SendBinary(byte[] data, int sendCode, int fromCode, int customCode,
            Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            SendFinal(data, 12, sendCode, fromCode, customCode, sockets);
        }

        // Header: 9
        public void SendId(int uniqueId, int sendCode, int fromCode, int customCode,
            Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            var id = ByteParser.ConvertObjectToBytes(uniqueId);

            SendFinal(id, 9, sendCode, fromCode, customCode, sockets);
        }

        // Header: 7
        public void SendInfoMessage(Socket sockets, string message, int target)
        {
            var data = ByteParser.ConvertObjectToBytes(message);
            byte headerByte = 7;
            var sendCodeByte = (byte) target;

            var newData = new byte[data.Length + 5];
            for (var i = 0; i < data.Length; i++)
                newData[i + 5] = data[i];
            newData[0] = headerByte;
            newData[1] = sendCodeByte;

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];

            sockets.BeginSend(newData, 0, newData.Length, SocketFlags.None, SendCallback, sockets);
        }

        // Header: 6
        public void SendFailure(Socket sockets, int target)
        {
            var data = ByteParser.ConvertObjectToBytes("Failed");
            byte headerByte = 6;
            var sendCodeByte = (byte) target;

            var newData = new byte[data.Length + 5];
            for (var i = 0; i < data.Length; i++)
                newData[i + 5] = data[i];
            newData[0] = headerByte;
            newData[1] = sendCodeByte;
            newData[2] = 0;

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];

            sockets.BeginSend(newData, 0, newData.Length, SocketFlags.None, SendCallback, sockets);
        }

        // Header: 5
        public void SendInvalid(Socket sockets, int target)
        {
            var data = ByteParser.ConvertObjectToBytes("Invalid");
            byte headerByte = 5;
            var sendCodeByte = (byte) target;

            var newData = new byte[data.Length + 5];
            for (var i = 0; i < data.Length; i++)
                newData[i + 5] = data[i];
            newData[0] = headerByte;
            newData[1] = sendCodeByte;
            newData[2] = 0;

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];

            sockets.BeginSend(newData, 0, newData.Length, SocketFlags.None, SendCallback, sockets);
        }

        // Header: 4
        public void SendCommandResponse(string text, int sendCode, int fromCode, int customCode,
            Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            var data = ByteParser.ConvertObjectToBytes(text);
            SendFinal(data, 4, sendCode, fromCode, customCode, sockets);
        }

        // Header: 3
        public void SendUserDataSetting(byte[] data, int user, Socket socket)
        {
            SendFinal(data, 3, 2, 0, 0, socket);
        }

        // Header: 2
        public void SendServerDataSetting(byte[] data, Socket socket)
        {
            SendFinal(data, 2, 2, 0, 0, socket);
        }

        // Header: 1
        public void SendServerData(byte[] data, Dictionary<Socket, MasterNetworkPlayer> sockets, int sendcode = 2)
        {
            SendFinal(data, 1, sendcode, 0, 0, sockets);
        }

        // Re-route
        public void Send(byte[] data, int header, int sendCode, int fromCode, int customCode,
            Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            SendFinal(data, header, sendCode, fromCode, customCode, sockets);
        }

        public void SendFinal(byte[] data, int header, int sendCode, int fromCode, int customCode,
            Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            var targetSockets = new List<Socket>();

            // Only Specific Client
            if (sendCode != 2 && sendCode != 1)
                targetSockets.Add(sockets.FirstOrDefault(x => x.Value.id == sendCode).Key);
            // Master client
            else if (sendCode == 1)
                targetSockets.Add(sockets.FirstOrDefault(x => x.Value.id != fromCode).Key);
            // Only All clients except to the from code
            else
                for (var i = 0; i < sockets.Count; i++)
                    if (sockets.Values.ElementAt(i).id != fromCode)
                        targetSockets.Add(sockets.Keys.ElementAt(i));

            targetSockets.ForEach(x =>
            {
                if (x == null || !Network.IsConnected(x) || targetSockets.Count <= 0) return;
            });

            if (targetSockets.Contains(null))
                return;

            var headerByte = (byte) header;
            var sendCodeByte = (byte) sendCode;

            var newData = new byte[data.Length + 5];
            for (var i = 0; i < data.Length; i++)
                newData[i + 5] = data[i];
            newData[0] = headerByte;
            newData[1] = sendCodeByte;
            newData[2] = 0;

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];

            targetSockets.ForEach(x =>
            {
                try
                {
                    x.BeginSend(newData, 0, newData.Length, SocketFlags.None, SendCallback, x);
                }
                catch (SocketException e)
                {
                    //Console.WriteLine(e.ToString());
                }
            } );
        }

        public void SendFinal(byte[] data, int header, int sendCode, int fromCode, int customCode, Socket socket)
        {
            if (!Network.IsConnected(socket)) return;

            var headerByte = (byte) header;
            var sendCodeByte = (byte) sendCode;

            var newData = new byte[data.Length + 5];
            for (var i = 0; i < data.Length; i++)
                newData[i + 5] = data[i];
            newData[0] = headerByte;
            newData[1] = sendCodeByte;
            newData[2] = 0;

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];

            socket.BeginSend(newData, 0, newData.Length, SocketFlags.None, SendCallback, socket);
        }

        private void SendCallback(IAsyncResult AR)
        {
            var socket = (Socket)AR.AsyncState;
            if (socket.Connected)
            {
                try
                {
                    socket.EndSend(AR);
                }
                catch (SocketException e)
                {

                }
            }
        }
    }

    public enum SendType
    {
        MasterClient,
        All
    }
}