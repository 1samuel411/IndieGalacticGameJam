using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterServerProj
{
    [Serializable]
    public class Game
    {

        public DateTime endTime; // Time allowed to complete the alert
        
        public Resource attitude;
        public Resource cabinPressure;
        public Resource speed;

        public Alert alert;

    }
}
