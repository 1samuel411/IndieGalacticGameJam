using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNetwork.Server
{
    public class Match
    {

        public int id;
        public string ip;
        public int port;
        public bool serverRunning = false;

        public List<string> rooms = new List<string>();
        public int eloAvg = 0;
        public bool started = false;
        public bool open = true;
        public DateTime startTime;

    }
}
