using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCommander : MonoBehaviour
{

    public Text attitude;
    public Text speed;
    public Text cabinPressure;

    void Start()
    {

    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        Room room = MasterClientManager.instance.GetRoom();

        if (room == null)
            return;

        string attitudeStr = room.game.game.attitude.value.ToString();
        string newAttitudeStr = "";
        for(int i = 0; i < attitudeStr.Length; i++)
        {
            newAttitudeStr += GetSymbol(int.Parse(attitudeStr[i].ToString()));
        }
        attitude.text = newAttitudeStr;

        string speedStr = room.game.game.speed.value.ToString();
        string newSpeedStr = "";
        for (int i = 0; i < speedStr.Length; i++)
        {
            newSpeedStr += GetSymbol(int.Parse(speedStr[i].ToString()));
        }
        speed.text = newSpeedStr;

        string cabinStr = room.game.game.cabinPressure.value.ToString();
        string newCabinStr = "";
        for (int i = 0; i < cabinStr.Length; i++)
        {
            newCabinStr += GetSymbol(int.Parse(cabinStr[i].ToString()));
        }
        cabinPressure.text = newCabinStr;
    }

    public static char[] characters = { 'c', 'a', 'o', '7', 'y', 'd', 't', 'k', '3', '}' };
    public char GetSymbol(int val)
    {

        if (val == 0)
        {
            return '☋';
        }
        if (val == 1)
        {
            return '☄';
        }
        if (val == 2)
        {
            return '≈';
        }
        if (val == 3)
        {
            return '☆';
        }
        if (val == 4)
        {
            return '☥';
        }
        if (val == 5)
        {
            return '♡';
        }
        if (val == 6)
        {
            return '☽';
        }
        if (val == 7)
        {
            return '☮';
        }
        if (val == 8)
        {
            return '☼';
        }
        if (val == 9)
        {
            return '☾';
        }
        return characters[val];
    }
}
