using System;
using System.Collections.Generic;
using UnityEngine;

namespace SNetwork
{
    [Serializable]
    public class Room
    {
        public string roomId;
        public bool matchmaking;
        public bool inMatch;
        public string matchId;
        public DateTime matchmakingBegin;
        public List<int> usersInRoomIds = new List<int>();
        public List<MasterNetworkPlayer> usersInRoom = new List<MasterNetworkPlayer>();
    }
}