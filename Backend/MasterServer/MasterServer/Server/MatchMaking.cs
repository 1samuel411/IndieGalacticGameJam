using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SNetwork.Server
{
    public class MatchMaking
    {

        public class MatchMakingGroup
        {
            public string roomId;
            public DateTime joinTime;
            public int elo;
            public int minRange;
            public int maxRange;
            public DateTime lastExpandTime;
        }

        public List<MatchMakingGroup> matchMakingSockets = new List<MatchMakingGroup>();

        public void AddToMatchMaking(Socket socketToAdd)
        {
            // Confirm the user is the room leader
            Room roomFound = ServerManager.instance.server.rooms.FirstOrDefault(x => x.usersInRoomIds.Contains(ServerManager.instance.server.clientSockets[socketToAdd].id));

            if(roomFound == null)
            {
                return;
            }

            if(roomFound.usersInRoomIds[0] != ServerManager.instance.server.clientSockets[socketToAdd].id)
            {
                return;
            }

            roomFound.Refresh();


            MatchMakingGroup group = new MatchMakingGroup();
            group.roomId = roomFound.roomId;
            int avgElo = 0;
            for(int i = 0; i < roomFound.usersInRoom.Count; i++)
            {
                Console.WriteLine("Adding to Matchmaking: " + roomFound.usersInRoom[i].username + ", elo: " + roomFound.usersInRoom[i].elo);
                avgElo += roomFound.usersInRoom[i].elo;
            }
            group.elo = avgElo / roomFound.usersInRoom.Count;
            group.joinTime = DateTime.UtcNow;
            group.lastExpandTime = DateTime.UtcNow;
            group.minRange = group.elo - 100;
            group.maxRange = group.elo + 100;
            matchMakingSockets.Add(group);

            roomFound.matchmaking = true;
            roomFound.matchmakingBegin = DateTime.UtcNow;
        }

        public void RemoveFromMatchMaking(Socket socketToRemove)
        {
            // Confirm the user is the room leader
            Room roomFound = ServerManager.instance.server.rooms.FirstOrDefault(x => x.usersInRoomIds.Contains(ServerManager.instance.server.clientSockets[socketToRemove].id));

            if (roomFound == null)
            {
                return;
            }

            if (roomFound.matchmaking == false)
            {
                return;
            }

            if (roomFound.usersInRoomIds[0] != ServerManager.instance.server.clientSockets[socketToRemove].id)
            {
                return;
            }

            matchMakingSockets.RemoveAll(x => x.roomId == roomFound.roomId);
            Messaging.instance.SendInfoMessage(socketToRemove, "Removed from matchmaking", 0);

            roomFound.matchmaking = false;
        }

        private Thread _userSyncThread;
        public MatchMaking()
        {
            _userSyncThread = new Thread(Begin);
            _userSyncThread.Start();
        }

        void Begin()
        {
            while(true)
            {
                Thread.Sleep(1000);
                MatchMake();    
            }
        }

        void MatchMake()
        {
            for (int i = 0; i < matchMakingSockets.Count; i++)
            {
                // Expand
                if((DateTime.Now - matchMakingSockets[i].lastExpandTime).Seconds >= 15)
                {
                    matchMakingSockets[i].lastExpandTime = DateTime.Now;
                    matchMakingSockets[i].maxRange += 100;
                    matchMakingSockets[i].minRange -= 100;
                }
            }

            for (int i = 0; i < matchMakingSockets.Count; i++)
            {
                for(int x = 0; x < ServerManager.instance.server.matchSockets.Count; x++)
                {
                    if (ServerManager.instance.server.matchSockets.Values.ElementAt(x).serverRunning == false)
                        continue;

                    if(ServerManager.instance.server.matchSockets.Values.ElementAt(x).rooms.Count <= 0 || ServerManager.instance.server.matchSockets.Values.ElementAt(x).eloAvg <= matchMakingSockets[i].maxRange || ServerManager.instance.server.matchSockets.Values.ElementAt(x).eloAvg >= matchMakingSockets[i].minRange)
                    {
                        ServerManager.instance.server.matchSockets.Values.ElementAt(x).rooms.Add(matchMakingSockets[i].roomId);
                        Room roomFound = ServerManager.instance.server.rooms.FirstOrDefault(p => p.roomId == matchMakingSockets[i].roomId);
                        if(roomFound != null)
                        {
                            roomFound.matchId = ServerManager.instance.server.matchSockets.Values.ElementAt(x).id;
                            roomFound.inMatch = true;
                            for(int p = 0; p < roomFound.usersInRoomIds.Count; p++)
                            {
                                Console.WriteLine("Match found for match: " + ServerManager.instance.server.matchSockets.Values.ElementAt(x).id + ", for room: " + roomFound.roomId);
                                Messaging.instance.SendMatchFound(ServerManager.instance.server.matchSockets.Values.ElementAt(x), roomFound.usersInRoomIds[p], ServerManager.instance.server.clientSockets);
                            }
                            matchMakingSockets.RemoveAt(i);
                        }
                    }
                }
            }
        }

    }
}
