using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class GameParent
{

    public bool beatLast = false;
    public int round;
    public int funding;
    public Game game;
    public bool ended;

    public ControllerInput inputA;
    public ControllerInput inputB;

}
