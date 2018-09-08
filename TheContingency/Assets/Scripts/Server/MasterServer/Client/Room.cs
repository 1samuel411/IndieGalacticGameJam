using System;
using System.Collections.Generic;
using UnityEngine;

namespace SNetwork
{
    [Serializable]
    public class Room
    {
        public string roomId;
		public bool startedGame;
        public List<int> usersInRoomIds = new List<int>();
        public List<MasterNetworkPlayer> usersInRoom = new List<MasterNetworkPlayer>();
    }
}