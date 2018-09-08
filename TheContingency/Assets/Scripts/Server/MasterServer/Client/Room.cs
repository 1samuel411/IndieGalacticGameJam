using SNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using SNetwork;

    [Serializable]
    public class Room
    {
        public string roomId;
		public bool startedGame;
        public List<int> usersInRoomIds = new List<int>();
        public List<MasterNetworkPlayer> usersInRoom = new List<MasterNetworkPlayer>();

        public GameParent game;
}