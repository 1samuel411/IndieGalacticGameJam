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
        public string username = "";
        public bool ready = false;
        public bool commander;
        public bool a;

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
