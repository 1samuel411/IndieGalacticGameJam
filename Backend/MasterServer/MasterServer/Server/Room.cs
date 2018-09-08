using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNetwork.Server
{
    public class Room
    {
        public string roomId;
        public bool matchmaking;
        public DateTime matchmakingBegin;
        public bool inMatch;
        public int matchId;
        public List<int> usersInRoomIds = new List<int>();
        public List<MasterNetworkPlayer> usersInRoom = new List<MasterNetworkPlayer>();

        public Room (int userId)
        {
            usersInRoomIds.Add(userId);
            roomId = Guid.NewGuid().ToString() + System.DateTime.Now.Millisecond.ToString();
        }

        public void Refresh()
        {
            // TODO: Optimize this
            usersInRoom.Clear();
            for (int i = 0; i < usersInRoomIds.Count; i++)
            {
                usersInRoom.Add(ServerManager.instance.server.clientSockets.Values.First(x => x.id == usersInRoomIds[i]));
            }
        }
    }
}
