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
        public string playfabId = "";
        public string username = "";
        public int elo = 0;
        public int xp = 0;
        public int playMode = 0;
        public bool inMatch;

        public string roomId
        {
            get
            {
                Room room = ServerManager.instance.server.rooms.FirstOrDefault(x => x.usersInRoomIds.Contains(id));
                if (room == null)
                    return "";
                else
                    return room.roomId;
            }
        }

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