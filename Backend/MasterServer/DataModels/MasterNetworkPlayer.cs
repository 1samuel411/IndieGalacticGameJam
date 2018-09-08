using System;
using System.Collections.Generic;
using System.Linq;
using SNetwork.Server;

namespace SNetwork
{
    [Serializable]
    public class MasterNetworkPlayer
    {
        public List<KeyValuePairs> data = new List<KeyValuePairs>();

        public int id = -1;
        public string username;
        public bool ready;

        public MasterNetworkPlayer(int id, string username)
        {
            this.id = id;
            this.username = username;
        }

        public MasterNetworkPlayer(int id)
        {
            this.id = id;
            username = "";
        }

        public MasterNetworkPlayer()
        {
            id = -1;
            username = "";
        }
    }
}