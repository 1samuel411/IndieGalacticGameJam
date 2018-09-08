using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace SNetwork
{
    public class MasterMessaging
    {
        private static MasterMessaging _instance;
        public static MasterMessaging instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MasterMessaging();
                }
                return _instance;
            }
        }

        // Header: 90
        public void SendNewPlayMode(int playMode, int sendCode, int fromCode, int customCode,
            Socket sockets)
        {
            byte[] data = ByteParser.ConvertASCIIToBytes(playMode.ToString());
            SendFinal(data, 90, sendCode, fromCode, customCode, sockets);
        }

        // Header: 77
        public void SendMatchMakeLeave(int sendCode, int fromCode, int customCode,
            Socket sockets)
        {
            // Send Match make Leave
            byte[] data = ByteParser.ConvertASCIIToBytes("a");
            SendFinal(data, 77, sendCode, fromCode, 0, sockets);
        }

        // Header: 76
        public void SendMatchMake(int sendCode, int fromCode, int customCode,
            Socket sockets)
        {
            // Send Matchmake
            byte[] data = ByteParser.ConvertASCIIToBytes("a");
            SendFinal(data, 76, sendCode, fromCode, 0, sockets);
        }

        // Header: 75
        public void SendLeave(int sendCode, int fromCode, int customCode,
            Socket sockets)
        {
            // Send Leave
            byte[] data = ByteParser.ConvertASCIIToBytes("a");
            SendFinal(data, 75, sendCode, fromCode, 0, sockets);
        }

        // Header: 74
        public void SendDecline(int inviteId, int sendCode, int fromCode, int customCode,
            Socket sockets)
        {
            // Send inviteId
            byte[] data = ByteParser.ConvertASCIIToBytes(inviteId.ToString());
            SendFinal(data, 74, sendCode, fromCode, 0, sockets);
        }

        // Header: 73
        public void SendAccept(int inviteId, int sendCode, int fromCode, int customCode,
            Socket sockets)
        {
            // Send inviteId
            byte[] data = ByteParser.ConvertASCIIToBytes(inviteId.ToString());
            SendFinal(data, 73, sendCode, fromCode, 0, sockets);
        }

        // Header: 72
        public void SendInvite(string playFabId, int sendCode, int fromCode, int customCode,
            Socket sockets)
        {
            // Send playfabid
            byte[] data = ByteParser.ConvertASCIIToBytes(playFabId);
            SendFinal(data, 72, sendCode, fromCode, 0, sockets);
        }

        // Header: 50
        public void SendMasterNetworkPlayer(MasterNetworkPlayer player, int sendCode, int fromCode, int customCode,
            Socket sockets)
        {
            byte[] data = ByteParser.ConvertNetworkPlayerToBytes(player);
            SendFinal(data, 50, sendCode, fromCode, customCode, sockets);
        }

        // Header: 21
        public void SendCommand(string text, int sendCode, int fromCode, int customCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            SendFinal(data, 21, sendCode, fromCode, customCode, sockets);
        }

        // Header: 20
        public void SendString(string text, int sendCode, int fromCode, int customCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            SendFinal(data, 20, sendCode, fromCode, customCode, sockets);
        }

        // Header: 14
        public void SendMasterNetworkPlayers(byte[] data, Dictionary<Socket, MasterNetworkPlayer> sockets, int sendcode = 2)
        {
            SendFinal(data, 14, 2, 0, 0, sockets);
        }

        // Header: 12
        public void SendBinary(byte[] data, int sendCode, int fromCode, int customCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            SendFinal(data, 12, sendCode, fromCode, customCode, sockets);
        }

        // Header: 9
        public void SendId(int uniqueId, int sendCode, int fromCode, int customCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            byte[] id;
            id = BitConverter.GetBytes(uniqueId);

            SendFinal(id, 9, sendCode, fromCode, customCode, sockets);
        }

        // Header: 7
        public void SendInfoMessage(Socket sockets, string message, int target)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            byte headerByte = (byte)(7);
            byte sendCodeByte = (byte)(target);

            byte[] newData = new byte[data.Length + 5];
            for (int i = 0; i < data.Length; i++)
            {
                newData[i + 5] = data[i];
            }
            newData[0] = headerByte;
            newData[1] = sendCodeByte;
            newData[2] = 0;

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];

            sockets.BeginSend(newData, 0, newData.Length, SocketFlags.None, new AsyncCallback(SendCallback), sockets);
        }

        // Header: 6
        public void SendFailure(Socket sockets, int target)
        {
            byte[] data = Encoding.ASCII.GetBytes("Failed");
            byte headerByte = (byte)(6);
            byte sendCodeByte = (byte)(target);

            byte[] newData = new byte[data.Length + 5];
            for (int i = 0; i < data.Length; i++)
            {
                newData[i + 5] = data[i];
            }
            newData[0] = headerByte;
            newData[1] = sendCodeByte;
            newData[2] = 0;

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];

            sockets.BeginSend(newData, 0, newData.Length, SocketFlags.None, new AsyncCallback(SendCallback), sockets);
        }

        // Header: 5
        public void SendInvalid(Socket sockets, int target)
        {
            byte[] data = Encoding.ASCII.GetBytes("Invalid");
            byte headerByte = (byte)(5);
            byte sendCodeByte = (byte)(target);

            byte[] newData = new byte[data.Length + 5];
            for (int i = 0; i < data.Length; i++)
            {
                newData[i + 5] = data[i];
            }
            newData[0] = headerByte;
            newData[1] = sendCodeByte;
            newData[2] = (byte)(0);

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];

            sockets.BeginSend(newData, 0, newData.Length, SocketFlags.None, new AsyncCallback(SendCallback), sockets);
        }

        // Header: 4
        public void SendCommandResponse(string text, int sendCode, int fromCode, int customCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
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
            SendFinal(data, 1, 2, 0, 0, sockets);
        }

        // Re-route
        public void Send(byte[] data, int header, int sendCode, int fromCode, int customCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            SendFinal(data, header, sendCode, fromCode, customCode, sockets);
        }

        public void SendFinal(byte[] data, int header, int sendCode, int fromCode, int customCode, Dictionary<Socket, MasterNetworkPlayer> sockets)
        {
            List<Socket> targetSockets = new List<Socket>();

            // Only Specific Client
            if (sendCode != 2 && sendCode != 1)
                targetSockets.Add(sockets.FirstOrDefault(x => x.Value.id == sendCode).Key);
            // Master client
            else if (sendCode == 1)
                targetSockets.Add(sockets.FirstOrDefault(x => x.Value.id != fromCode).Key);
            // Only All clients except to the from code
            else
            {
                // TODO: Optimize?
                for (int i = 0; i < sockets.Count; i++)
                {
                    if(sockets.Values.ElementAt(i).id != fromCode)
                        targetSockets.Add(sockets.Keys.ElementAt(i));
                }
            }

            targetSockets.ForEach(x =>
            {
                if (!Network.IsConnected(x) || targetSockets.Count <= 0) return;
            });

            byte headerByte = (byte)header;
            byte sendCodeByte = (byte)sendCode;

            byte[] newData = new byte[data.Length + 5];
            for (int i = 0; i < data.Length; i++)
            {
                newData[i + 5] = data[i];
            }
            newData[0] = headerByte;
            newData[1] = sendCodeByte;
            newData[2] = (byte)0;

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];
            
            targetSockets.ForEach(x =>
            {
                x.BeginSend(newData, 0, newData.Length, SocketFlags.None, new AsyncCallback(SendCallback), x);
            });
        }

        public void SendFinal(byte[] data, int header, int sendCode, int fromCode, int customCode, Socket socket)
        {
            if (!Network.IsConnected(socket)) return;

            byte headerByte = (byte)header;
            byte sendCodeByte = (byte)sendCode;

            byte[] newData = new byte[data.Length + 5];
            for (int i = 0; i < data.Length; i++)
            {
                newData[i + 5] = data[i];
            }
            newData[0] = headerByte;
            newData[1] = sendCodeByte;
            newData[2] = (byte)0;

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];

            socket.BeginSend(newData, 0, newData.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        }

        private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }

    public enum SendType
    {
        MasterClient,
        All
    }
}