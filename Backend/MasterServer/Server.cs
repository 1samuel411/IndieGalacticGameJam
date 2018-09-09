using MasterServerProj;
using MasterServerProj.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SNetwork.Server
{
    public class Server
    {
        public string serverRegion;

        private byte[] _buffer = new byte[50000];
        private int _bufferSize;

        public bool _opened;
		
		public int maxUsers = 999999;

        private Thread _userSyncThread;
        private float _userSyncTime;
        public Dictionary<Socket, MasterNetworkPlayer> clientSockets = new Dictionary<Socket, MasterNetworkPlayer>();

        public List<KeyValuePairs> serverData = new List<KeyValuePairs>();

        public List<Room> rooms = new List<Room>();

        public Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		
        private void BeginAccepting()
        {
            serverSocket.BeginAccept(AcceptedConnection, null);
        }

        private void BeginReceiving(Socket socket)
        {
            try
            {
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, socket);
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetupServer(string ip = "127.0.0.1", int port = 100, int bufferSize = 256000)
        {
            _bufferSize = bufferSize;
            _buffer = new byte[_bufferSize];
            Console.WriteLine("[SNetworking] Creating and seting up the server...");

            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            serverSocket.Listen(90);

            BeginAccepting();
            Console.WriteLine("[SNetworking] Success!");

            _opened = true;

            _userSyncThread = new Thread(Sync);
            _userSyncThread.Start();
        }

        private List<int> uniqueIdsToSync = new List<int>();
        private void Sync()
        {
            var iteration = 0;
            while (_opened)
            {
                iteration++;

                Thread.Sleep((int) (200));

                // Send rooms to users
                List<int> usersGotit = new List<int>();
                for(int i = 0; i < rooms.Count; i++)
                {
                    for(int x = 0; x < rooms[i].usersInRoomIds.Count; x++)
                    {
                        usersGotit.Add(rooms[i].usersInRoomIds[x]);
                        Messaging.instance.SendRoom(rooms[i], rooms[i].usersInRoomIds[x], clientSockets);
                    }
                }

                Thread.Sleep((int)(200));

                for (int i = 0; i < clientSockets.Count; i++)
                {
                    if(!usersGotit.Contains(clientSockets.ElementAt(i).Value.id))
                    {
                        Messaging.instance.SendRoom(null, clientSockets.ElementAt(i).Value.id, clientSockets);
                    }
                }

                Thread.Sleep((int)(200));

                for(int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].startedGame)
                    {
                        if ((rooms[i].game.funding <= 0 || DateTime.UtcNow > rooms[i].game.game.endTime) && !rooms[i].game.ended)
                        {
                            rooms[i].game.ended = true;
                            for (int x = 0; x < rooms[i].usersInRoom.Count; x++)
                            {
                                Socket socket = clientSockets.FirstOrDefault(p => p.Value.id == rooms[i].usersInRoom[x].id).Key;
                                Messaging.instance.SendInfoMessage(socket, "Over", 0);
                            }
                        }

                        if (rooms[i].game.round >= 7)
                        {
                            continue;
                        }

                        if (rooms[i].game.funding <= 0)
                        {
                            continue;
                        }

                        if (rooms[i].game.ended && rooms[i].game.inputA != null && rooms[i].game.inputB != null)
                        {
                            rooms[i].game.ended = false;
                            Console.WriteLine("Ended Room: " + rooms[i].roomId);

                            Console.WriteLine("  Input A (Pressure): " + rooms[i].game.inputA.pressure);
                            Console.WriteLine("  Input A (Speed): " + rooms[i].game.inputA.speed);
                            Console.WriteLine("  Input A (Attitude): " + rooms[i].game.inputA.attitude);

                            Console.WriteLine("  Input B (Pressure): " + rooms[i].game.inputB.pressure);
                            Console.WriteLine("  Input B (Speed): " + rooms[i].game.inputB.speed);
                            Console.WriteLine("  Input B (Attitude): " + rooms[i].game.inputB.attitude);

                            string attitudeStr = "";
                            for (int p = 0; p < rooms[i].game.game.attitude.symbol.Count; p++)
                            {
                                attitudeStr += rooms[i].game.game.attitude.symbol[p].value;
                            }
                            int attitudeNew = (int.Parse(attitudeStr));

                            attitudeNew += rooms[i].game.inputA.attitude;

                            if (attitudeNew < 0)
                                attitudeNew *= -1;

                            string symbolsToGenerateStr = attitudeNew.ToString();
                            List<Symbol> symbols = new List<Symbol>();
                            for (int p = 0; p < symbolsToGenerateStr.Length; p++)
                            {
                                int x = int.Parse(symbolsToGenerateStr[p].ToString());
                                Symbol symbol = new Symbol();
                                symbol.unicodeValue = Symbol.characters[x].ToString();
                                symbol.value = x;
                                symbols.Add(symbol);
                            }
                            rooms[i].game.game.attitude.symbol = symbols;

                            attitudeStr = "";
                            for (int p = 0; p < rooms[i].game.game.cabinPressure.symbol.Count; p++)
                            {
                                attitudeStr += rooms[i].game.game.cabinPressure.symbol[p].value;
                            }
                            attitudeNew = (int.Parse(attitudeStr));

                            attitudeNew += rooms[i].game.inputA.pressure;

                            if (attitudeNew < 0)
                                attitudeNew *= -1;

                            symbolsToGenerateStr = attitudeNew.ToString();
                            symbols = new List<Symbol>();
                            for (int p = 0; p < symbolsToGenerateStr.Length; p++)
                            {
                                int x = int.Parse(symbolsToGenerateStr[p].ToString());
                                Symbol symbol = new Symbol();
                                symbol.unicodeValue = Symbol.characters[x].ToString();
                                symbol.value = x;
                                symbols.Add(symbol);
                            }
                            rooms[i].game.game.cabinPressure.symbol = symbols;

                            attitudeStr = "";
                            for (int p = 0; p < rooms[i].game.game.speed.symbol.Count; p++)
                            {
                                attitudeStr += rooms[i].game.game.speed.symbol[p].value;
                            }
                            attitudeNew = (int.Parse(attitudeStr));

                            attitudeNew += rooms[i].game.inputA.speed;

                            if (attitudeNew < 0)
                                attitudeNew *= -1;

                            symbolsToGenerateStr = attitudeNew.ToString();
                            symbols = new List<Symbol>();
                            for (int p = 0; p < symbolsToGenerateStr.Length; p++)
                            {
                                int x = int.Parse(symbolsToGenerateStr[p].ToString());
                                Symbol symbol = new Symbol();
                                symbol.unicodeValue = Symbol.characters[x].ToString();
                                symbol.value = x;
                                symbols.Add(symbol);
                            }
                            rooms[i].game.game.speed.symbol = symbols;

                            attitudeStr = "";
                            for (int p = 0; p < rooms[i].game.game.attitude.symbol.Count; p++)
                            {
                                attitudeStr += rooms[i].game.game.attitude.symbol[p].value;
                            }
                            attitudeNew = (int.Parse(attitudeStr));

                            attitudeNew += rooms[i].game.inputB.attitude;

                            if (attitudeNew < 0)
                                attitudeNew *= -1;

                            symbolsToGenerateStr = attitudeNew.ToString();
                            symbols = new List<Symbol>();
                            for (int p = 0; p < symbolsToGenerateStr.Length; p++)
                            {
                                int x = int.Parse(symbolsToGenerateStr[p].ToString());
                                Symbol symbol = new Symbol();
                                symbol.unicodeValue = Symbol.characters[x].ToString();
                                symbol.value = x;
                                symbols.Add(symbol);
                            }
                            rooms[i].game.game.attitude.symbol = symbols;

                            attitudeStr = "";
                            for (int p = 0; p < rooms[i].game.game.cabinPressure.symbol.Count; p++)
                            {
                                attitudeStr += rooms[i].game.game.cabinPressure.symbol[p].value;
                            }
                            attitudeNew = (int.Parse(attitudeStr));

                            attitudeNew += rooms[i].game.inputB.pressure;

                            if (attitudeNew < 0)
                                attitudeNew *= -1;

                            symbolsToGenerateStr = attitudeNew.ToString();
                            symbols = new List<Symbol>();
                            for (int p = 0; p < symbolsToGenerateStr.Length; p++)
                            {
                                int x = int.Parse(symbolsToGenerateStr[p].ToString());
                                Symbol symbol = new Symbol();
                                symbol.unicodeValue = Symbol.characters[x].ToString();
                                symbol.value = x;
                                symbols.Add(symbol);
                            }
                            rooms[i].game.game.cabinPressure.symbol = symbols;

                            attitudeStr = "";
                            for (int p = 0; p < rooms[i].game.game.speed.symbol.Count; p++)
                            {
                                attitudeStr += rooms[i].game.game.speed.symbol[p].value;
                            }
                            attitudeNew = (int.Parse(attitudeStr));

                            attitudeNew += rooms[i].game.inputB.speed;

                            if (attitudeNew < 0)
                                attitudeNew *= -1;

                            symbolsToGenerateStr = attitudeNew.ToString();
                            symbols = new List<Symbol>();
                            for (int p = 0; p < symbolsToGenerateStr.Length; p++)
                            {
                                int x = int.Parse(symbolsToGenerateStr[p].ToString());
                                Symbol symbol = new Symbol();
                                symbol.unicodeValue = Symbol.characters[x].ToString();
                                symbol.value = x;
                                symbols.Add(symbol);
                            }
                            rooms[i].game.game.speed.symbol = symbols;

                            bool beatLast = false;

                            if(rooms[i].game.game.alert.resource.name == "Attitude")
                            {
                                attitudeStr = "";
                                for (int x = 0; x < rooms[i].game.game.attitude.symbol.Count; x++)
                                {
                                    attitudeStr += rooms[i].game.game.attitude.symbol[x].value;
                                }
                                int attitudeVal = (int.Parse(attitudeStr));
                                
                                if(attitudeVal == rooms[i].game.game.alert.targetResourceValue)
                                {
                                    beatLast = true;
                                }
                            }
                            else if(rooms[i].game.game.alert.resource.name == "Cabin Pressure")
                            {
                                attitudeStr = "";
                                for (int x = 0; x < rooms[i].game.game.cabinPressure.symbol.Count; x++)
                                {
                                    attitudeStr += rooms[i].game.game.cabinPressure.symbol[x].value;
                                }
                                int attitudeVal = (int.Parse(attitudeStr));

                                if (attitudeVal == rooms[i].game.game.alert.targetResourceValue)
                                {
                                    beatLast = true;
                                }
                            }
                            else if(rooms[i].game.game.alert.resource.name == "Speed")
                            {
                                attitudeStr = "";
                                for (int x = 0; x < rooms[i].game.game.speed.symbol.Count; x++)
                                {
                                    attitudeStr += rooms[i].game.game.speed.symbol[x].value;
                                }
                                int attitudeVal = (int.Parse(attitudeStr));

                                if (attitudeVal == rooms[i].game.game.alert.targetResourceValue)
                                {
                                    beatLast = true;
                                }
                            }

                            rooms[i].game.inputA = null;
                            rooms[i].game.inputA = null;

                            Random rand = new Random();
                            if(beatLast)
                            {
                                rooms[i].game.funding += rand.Next(20000, 30000);
                            }
                            else
                            {
                                rooms[i].game.funding -= rand.Next(40000, 60000);
                            }

                            rooms[i].game.beatLast = beatLast;
                            rooms[i].game.game = rooms[i].game.GenerateGame();
                            rooms[i].game.round++;
                        }
                    }
                }
            }
        }

        public void CloseServer()
        {
            for (var i = 0; i < clientSockets.Count; i++)
            {
                var clientSocket = clientSockets.Keys.ElementAt(i);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }

            clientSockets.Clear();
            serverSocket.Close();

            _opened = false;

            Console.WriteLine("[SNetworking] Server successfully closed!");
        }

        private void AcceptedConnection(IAsyncResult AR)
        {
            if (serverSocket == null)
                return;
            Socket socket = null;
            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException e)
            {

            }
            if (socket == null)
                return;

            if (clientSockets.Count >= maxUsers)
            {
                Console.WriteLine("[SNetworking] Max clients reached");
                Messaging.instance.SendInfoMessage(socket, "Full", 0);
                RemoveSocket(socket);
                return;
            }

            Console.WriteLine("[SNetworking] Connection received from: " + socket.LocalEndPoint);

            var uniqueId = new Random().Next(3, maxUsers + 5);

            Console.WriteLine("Checking unique id: " + uniqueId);

            if (clientSockets.Count > 0)
            {
                var unique = false;
                var changed = false;
                while (!unique)
                {
                    changed = false;
                    // TODO: Make this more optimized
                    foreach (var x in clientSockets.Values)
                        if (x.id == uniqueId)
                        {
                            changed = true;
                            uniqueId = new Random().Next(3, maxUsers + 5);
                            Console.WriteLine("Checking unique id: " + uniqueId);
                        }
                    if (!changed)
                        unique = true;
                }
            }

            Console.WriteLine("Assigning unique id: " + uniqueId);
            clientSockets.Add(socket, new MasterNetworkPlayer(uniqueId));
            uniqueIdsToSync.Add(uniqueId);

            BeginReceiving(socket);
            BeginAccepting();

            Messaging.instance.SendId(uniqueId, uniqueId, 0, 0, clientSockets);
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            var socket = (Socket)AR.AsyncState;
            if (!IsConnectedServer(socket))
                return;

            var received = socket.EndReceive(AR);

            var dataBuffer = new byte[received];
            Array.Copy(_buffer, dataBuffer, received);

            if (dataBuffer.Length == 1)
            {
                Console.WriteLine("Connected?");
                return;
            }
            var headerCode = Convert.ToInt32(dataBuffer[0]);
            var customCode = new byte[2];
            customCode[0] = dataBuffer[3];
            customCode[1] = dataBuffer[4];
            var sendCode = Convert.ToInt32(dataBuffer[1]);
            if (headerCode == 0)
                Messaging.instance.Send(dataBuffer.Skip(5).Take(BitConverter.ToInt16(customCode, 0)).ToArray(), headerCode, sendCode, clientSockets[socket].id,
                    BitConverter.ToInt16(customCode, 0), clientSockets);
            else
                ResponseManager.instance.HandleResponse(dataBuffer.Skip(5).Take(BitConverter.ToInt16(customCode, 0)).ToArray(), headerCode, sendCode,
                    0, socket, clientSockets[socket].id);

            if (socket.Connected)
                BeginReceiving(socket);
        }

        public void SetServerData(KeyValuePairs data)
        {
            serverData = SetData(serverData, data);
        }

        public void SetUserData(int target, KeyValuePairs data)
        {
            for (var i = 0; i < clientSockets.Count; i++)
                if (clientSockets.Values.ElementAt(i).id == target)
                {
                    clientSockets.Values.ElementAt(i).data = SetData(clientSockets.Values.ElementAt(i).data, data);
                    return;
                }
        }

        private List<KeyValuePairs> SetData(List<KeyValuePairs> source, KeyValuePairs data)
        {
            for (var i = 0; i < source.Count; i++)
                if (source[i].Key == data.Key)
                {
                    source[i] = data;
                    return source;
                }
            source.Add(data);
            return source;
        }

        public bool IsConnectedServer(Socket socket)
        {
            var isConnected = Network.IsConnected(socket);
            if (!isConnected)
            {
                var socketRetrieved = clientSockets.FirstOrDefault(t => t.Key == socket);
                Console.WriteLine("[SNetworking] MasterNetworkPlayer: " + socketRetrieved.Value.id +
                                  ", has been lost.");
                RemoveSocket(socket);
            }
            return isConnected;
        }

        public void RemoveSocket(Socket socket)
        {

            var socketRetrieved = clientSockets.FirstOrDefault(t => t.Key == socket);
            try
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (ObjectDisposedException e)
                {

                }
            }
            catch (SocketException e)
            {

            }

            Room room = GetRoom(clientSockets[socket].id);
            if (room != null)
            {
                LeaveRoom(socket);
            }
            clientSockets.Remove(socket);
        }

        private static void SendCallback(IAsyncResult AR)
        {
            var socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }

        public void JoinRoom(Socket fromUser, string idToJoin)
        {
            Room roomToJoin = rooms.FirstOrDefault(x => x.roomId == idToJoin);

            if(roomToJoin != null)
            {
                Console.WriteLine("Found room!");

                if (roomToJoin.usersInRoomIds.Count > 2)
                {
                    // Full
                    Messaging.instance.SendInfoMessage(fromUser, "[Full Room]", 0);
                    return;
                }
                roomToJoin.usersInRoomIds.Add(clientSockets[fromUser].id);
                roomToJoin.Refresh();
            }
            else
            {
                Messaging.instance.SendInfoMessage(fromUser, "[Not Exist]", 0);
                Console.WriteLine("Could not find room!");
            }
        }

        private Random random = new Random();
        public void CreateRoom(Socket fromSocket)
        {
            MasterNetworkPlayer masterNetworkPlayer = clientSockets[fromSocket];

            Room room = new Room(masterNetworkPlayer.id);
            room.roomId = random.Next(1, 9) + "" + random.Next(0, 9) + "" + random.Next(0, 9) + "" + random.Next(0, 9).ToString();
            while(rooms.Count(x=>x.roomId == room.roomId) > 0)
            {
                room.roomId = random.Next(1, 9) + "" + random.Next(0, 9) + "" + random.Next(0, 9) + "" + random.Next(0, 9).ToString();
            }
            rooms.Add(room);
        }

        public void LeaveRoom(Socket socket)
        {
            Room room = GetRoom(clientSockets[socket].id);

            if(room != null)
            {
                room.usersInRoomIds.Remove(clientSockets[socket].id);
                room.Refresh();
                if(room.usersInRoomIds.Count <= 0 || room.startedGame)
                    DeleteRoom(room.roomId);
            }
        }

        private Room GetRoom(int userId)
        {
            return rooms.FirstOrDefault(x => x.usersInRoomIds.Contains(userId) == true);
        }

        private void DeleteRoom(string roomId)
        {
            Room roomToRemove = rooms.FirstOrDefault(x => x.roomId == roomId);

            if(roomToRemove != null)
            {
                rooms.Remove(roomToRemove);
            }
        }

        public void ToggleReady(Socket userId)
        {
            MasterNetworkPlayer player = clientSockets[userId];

            player.ready = !player.ready;

            Console.WriteLine("Ready toggled! " + player.ready);

            Room room = GetRoom(player.id);
            room.Refresh();
            bool allReady = true;
            for(int i = 0; i < room.usersInRoom.Count; i++)
            {
                if (room.usersInRoom[i].ready == false)
                    allReady = false;
            }

            if (room.usersInRoom.Count < 3)
                allReady = false;

            if(allReady)
            {
                StartGame(room);
            }
        }

        public void StartGame(Room room)
        {
            Console.WriteLine("Starting Room: " + room.roomId);
            room.startedGame = true;
            room.game = new GameParent();

            room.Refresh();

            int randomUser = random.Next(0, 3);

            room.usersInRoom[randomUser].commander = true;

            for (int i = 0; i < room.usersInRoom.Count; i++)
            {
                if (!room.usersInRoom[i].commander)
                {
                    room.usersInRoom[i].a = true;
                    break;
                }
            }

            room.game.round = 0;
            room.game.funding = 100000;
            room.game.game = room.game.GenerateGame();
        }

        public void SetInput(Socket socket, ControllerInput input)
        {
            Room room = GetRoom(clientSockets[socket].id);

            if (room == null)
                return;

            if(clientSockets[socket].a)
            {
                room.game.inputA = input;
            }
            else
            {
                room.game.inputB = input;
            }
        }
    }
}