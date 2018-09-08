using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[Serializable]
public class Game
{

    public DateTime endTime;

    public Resource attitude;
    public Resource cabinPressure;
    public Resource speed;

    public int targetAttitude;
    public int targetCabinPressure;
    public int targetSpeed;

    public Alert alert;
    
}
