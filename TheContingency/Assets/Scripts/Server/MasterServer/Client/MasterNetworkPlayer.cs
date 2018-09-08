using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SNetwork
{
    [Serializable]
	public class MasterNetworkPlayer
	{
	    public List<KeyValuePairs> data = new List<KeyValuePairs>();

        public int id = -1;
        public string playfabId = "";
        public string username = "";
        public string roomId = "";
        public int elo = 0;
        public int xp = 0;
        public int playMode = 0;
        public bool inMatch;

        public MasterNetworkPlayer(int id, string username)
        {
            this.id = id;
            this.username = username;
        }

        public MasterNetworkPlayer(int id)
        {
            this.id = id;
            this.username = "";
        }

        public MasterNetworkPlayer()
        {
            this.id = -1;
            this.username = "";
        }
	}
}
